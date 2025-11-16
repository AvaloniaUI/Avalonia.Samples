# Rect Painter Sample

<!-- Write a short summary here what this examples does -->
This example will show you how to create a custom rendered control which interacts with the mouse, to form a simple paint application


### Difficulty
<!-- Choose one of the below difficulties. You can just delete the ones you don't need. -->

🐔 Normal 🐔



### Buzz-Words

<!-- Write some buzz-words here. You can separate them by ", " -->
Graphics Editor, Paint, MVVM



## The Solution

There is a custom control `PaintControl` and a corresponding view model for that control `PaintControlViewModel`.

The view model holds data for the control as properties, and also holds the off-screen image which is being edited. 

The `PaintControl` :- 

* responds to mouse movement, mouse button presses, and also to global keyboard events
* maintains information about the editing process, what rectangle you are dragging out, and what color it is, in the view model.
* requests the view model to modify the image when you create a new rectangle, or when the image size needs to change in response to the application resizing.

The `PaintControlViewModel` :-

* publishes the properties it holds using the `INotifyPropertyChanged` interface.
* handles the actual editing of the image.

The `MainWindowViewModel` :- 

* subscribes to properties in the `PaintControlViewModel` so that it can provide properties to the UI for binding

## Notes
<!-- Any related information or further readings goes here. -->

The `PaintControlViewModel` holds a single image that is being edited, and the `PaintControl` renders that image when needed. Editing the image involves creating a new image, painting the old one onto it, and then adding the newly requested rectangle.

More control and editing capabilities could be achieved using a dedicated graphics library such as Skia.
