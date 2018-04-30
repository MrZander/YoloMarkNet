# YoloMarkNet

WIP, inspired by https://github.com/AlexeyAB/Yolo_mark
I wanted to be able to delete regions, so I made this.

![Example](https://i.imgur.com/fpNW1TQ.jpg)

1.) Build the project

2.) Place images into Debug/data/img or Release/data/img depending on what config you built under

3.) Put your class names into obj.names and update obj.data to have the correct number of classes

4.) Run YoloMarkNet.exe

5.) Select your class from the panel on the left

6.) Highlight regions in the images

You can view AlexeyAB's repos for instructions on how to train with the generated data

It needs some work still, I plan to add more features:
* Handling scaling better.  (Entire canvas is scaled right now, doesn't work well with large or tiny images)
* Select regions point-by-point and find a best fitting rect
* Other output formats
