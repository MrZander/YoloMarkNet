using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace YoloMarkNet
{

    //Adapted from https://www.codeproject.com/Articles/22952/WPF-Diagram-Designer-Part
    public class BoundingThumb : Thumb
    {

        public BoundingThumb()
        {
            DragDelta += new DragDeltaEventHandler(OnDragDelta);
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var item = DataContext as Control;
            if (item != null)
            {
                double left = Canvas.GetLeft(item);
                double top = Canvas.GetTop(item); 
                Canvas.SetLeft(item, left + e.HorizontalChange);
                Canvas.SetTop(item, top + e.VerticalChange);
            }
        }

    }
}
