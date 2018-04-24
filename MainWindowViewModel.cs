using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace YoloMarkNet
{

    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;
        Action<object> action;
        public Command(Action<object> action)
        {
            this.action = action;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action(parameter);
        }
    }

    public class Notifier : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName()]string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }

    public class Image : Notifier
    {
        private bool _isSelected;
        public string Path { get; set; }
        public ImageSource Thumb { get; set; }
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; NotifyPropertyChanged(); }
        }
    }

    public class BoundingBox : Notifier
    {
        public BoundingBox(Rect rect) { this.rect = rect; }
        private Rect rect;
        public double X
        {
            get { return rect.X; }
            set
            {
                rect.X = value;
                NotifyPropertyChanged();
            }
        }
        public double Y
        {
            get { return rect.Y; }
            set
            {
                rect.Y = value;
                NotifyPropertyChanged();
            }
        }
        public double Width
        {
            get { return rect.Width; }
            set
            {
                rect.Width = value;
                NotifyPropertyChanged();
            }
        }
        public double Height
        {
            get { return rect.Height; }
            set
            {
                rect.Height = value;
                NotifyPropertyChanged();
            }
        }
    }

    public class MainWindowViewModel : Notifier
    {

        private ImageSource _selectedImageSource;
        private string _selectedClass;
        private double _mouseX;
        private double _mouseY;
        private bool _isMouseDown;

        public bool IsMouseDown
        {
            get { return _isMouseDown; }
            set { _isMouseDown = value; NotifyPropertyChanged(); }
        }

        public double MouseY
        {
            get { return _mouseY; }
            set { _mouseY = value; NotifyPropertyChanged(); }
        }

        public double MouseX
        {
            get { return _mouseX; }
            set { _mouseX = value; NotifyPropertyChanged(); }
        }

        public IEnumerable<string> Classes { get; }

        public IEnumerable<Image> Images { get; }

        public ObservableCollection<BoundingBox> BoundingBoxes { get; } = new ObservableCollection<BoundingBox>();

        public Image SelectedImage
        {
            get { return Images.FirstOrDefault(f => f.IsSelected); }
            set
            {
                SelectedImageSource = null;
                foreach(var image in Images)
                {
                    if (image == value)
                    {
                        image.IsSelected = true;
                        SelectedImageSource = LoadImage(image.Path);
                    }
                    else
                    {
                        image.IsSelected = false;
                    }
                }
                NotifyPropertyChanged();
            }
        }
        
        public ImageSource SelectedImageSource
        {
            get { return _selectedImageSource; }
            set { _selectedImageSource = value; NotifyPropertyChanged(); }
        }
        
        public string SelectedClass
        {
            get { return _selectedClass; }
            set { _selectedClass = value; NotifyPropertyChanged(); }
        }
        
        public MainWindowViewModel()
        {

            //Fix design time errors
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject())) return;
             
            Classes = System.IO.File.ReadAllText("data\\obj.names")
                                    .Split('\n')
                                    .Select(f => f.Trim())
                                    .ToList();

            Images = System.IO.Directory.GetFiles("data\\img", "*.jpg").Select(f => new Image()
            {
                Path = f,
                Thumb = LoadImage(f, true)
            }).ToList();

            SelectedImage = Images.FirstOrDefault();
            if(SelectedImage == null)
            {
                MessageBox.Show("No images found, please place images into data/img/");
                Application.Current.MainWindow.Close();
                return;
            }
            if(Classes.Count() == 0)
            {
                MessageBox.Show("No classes found, please enter classes into data/obj.names");
                Application.Current.MainWindow.Close();
                return;
            }

        }

        private ImageSource LoadImage(string path, bool thumb = false)
        {
            var src = new BitmapImage();
            using (FileStream stream = File.OpenRead(path))
            {
                src.BeginInit();
                src.StreamSource = stream;
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit(); 
            } 
            if (thumb)
            {
                var scaleX = 128f / src.PixelWidth;
                var scaleY = 128f / src.PixelHeight;
                return new TransformedBitmap(src, new ScaleTransform(scaleX, scaleY));
            }
            return src;

        }

        public ICommand SelectImageCommand => new Command(p =>
        {
            SelectedImage = (Image)p;
        });

        public void MouseMove(Point point)
        {
            MouseX = point.X;
            MouseY = point.X;
        }

        private Point mouseDownLocation;
        public void MouseDown(Point point)
        {
            IsMouseDown = true;
            mouseDownLocation = point;
        }

        public void MouseUp(Point point)
        {
            IsMouseDown = false;
            var rect = new Rect(mouseDownLocation, point);
            BoundingBoxes.Add(new BoundingBox(rect));
        }

    }

}
