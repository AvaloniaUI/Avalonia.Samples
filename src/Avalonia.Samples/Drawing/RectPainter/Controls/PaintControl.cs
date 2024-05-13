using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using RectPainter.ViewModels;

namespace RectPainter.Controls
{
    /// <summary>
    /// A control that responds to mouse and keyboard, to edit and render an image
    /// </summary>
    public class PaintControl : Control
    {
        public PaintControlViewModel? Vm => DataContext as PaintControlViewModel;

        public PaintControl()
        {
            // Setup event handlers
            PointerMoved += PaintControl_PointerMoved;
            PointerPressed += PaintControl_PointerPressed;
            PointerReleased += PaintControl_PointerReleased;
            PointerCaptureLost += PaintControl_PointerCaptureLost;
            SizeChanged += PaintControl_SizeChanged;

            KeyDownEvent.AddClassHandler<TopLevel>(PaintControl_KeyDown, handledEventsToo: true);
            KeyUpEvent.AddClassHandler<TopLevel>(PaintControl_KeyUp, handledEventsToo: true);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == DataContextProperty && Bounds.Size != default)
            {
                // let the view model know the PaintControl's size, so the image can be the correct size.
                Vm?.SetImageSize(new PixelSize((int)Bounds.Width, (int)Bounds.Height));

                // Request the image be rendered
                InvalidateVisual();
            }
        }

        private void PaintControl_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            if (Vm != null)
            {
                // Update the mouse position, and the currently selected marquee
                var pos = e.GetPosition(this);
                Vm.Pos = pos;
                if (Vm.Dragging)
                {
                    Vm.Marquee = new Rect(
                        System.Math.Min(Vm.Origin.X, Vm.Pos.X),
                        System.Math.Min(Vm.Origin.Y, Vm.Pos.Y),
                        System.Math.Abs(Vm.Origin.X - Vm.Pos.X),
                        System.Math.Abs(Vm.Origin.Y - Vm.Pos.Y));
                    InvalidateVisual();
                }
                else
                {
                    Vm.Origin = pos;
                }
                e.Handled = true;
            }
        }

        private void PaintControl_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (Vm != null)
            {
                // Start the drag
                Vm.Dragging = true;
            }
        }

        private void PaintControl_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            if (Vm != null)
            {
                if (Vm.Dragging == true)
                {
                    // Finish dragging
                    Vm.Dragging = false;

                    // Paint a new rectangle
                    Vm.AddRectangle();

                    // Request the updated image be rendered
                    InvalidateVisual();
                }
            }
        }

        private void PaintControl_PointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
        {
            if (Vm != null)
            {
                // finish Dragging
                Vm.Dragging = false;

                // Request the image be rendered (to clear any marquee)
                InvalidateVisual();
            }
        }

        private void PaintControl_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            if (Vm != null)
            {
                // Make sure the image matches the size of the control
                Vm.SetImageSize(new PixelSize((int)e.NewSize.Width, (int)e.NewSize.Height));

                // Request the updated image be rendered
                InvalidateVisual();
            }
        }

        private void PaintControl_KeyDown(object? sender, KeyEventArgs e)
        {
            if (Vm != null)
            {
                // Change rectangle color or cancel dragging
                // Request the updated image be rendered, in case there is a marquee
                switch (e.Key)
                {
                    case Key.LeftShift:
                        Vm.Red = 255;
                        InvalidateVisual();
                        break;

                    case Key.LeftCtrl:
                        Vm.Green = 255;
                        InvalidateVisual();
                        break;

                    case Key.LeftAlt:
                        Vm.Blue = 255;
                        InvalidateVisual();
                        break;

                    case Key.Escape:
                        Vm.Dragging = false;
                        InvalidateVisual();
                        break;
                }
            }
        }

        private void PaintControl_KeyUp(object? sender, KeyEventArgs e)
        {
            if (Vm != null)
            {
                // Change rectangle color
                // Request the updated image be rendered, in case there is a marquee
                switch (e.Key)
                {
                    case Key.LeftShift:
                        Vm.Red = 0;
                        InvalidateVisual();
                        break;

                    case Key.LeftCtrl:
                        Vm.Green = 0;
                        InvalidateVisual();
                        break;

                    case Key.LeftAlt:
                        Vm.Blue = 0;
                        InvalidateVisual();
                        break;
                }
            }
        }


        /// <summary>
        /// Render the saved graphic, and marquee when needed
        /// </summary>
        /// <param name="context"></param>
        public override void Render(DrawingContext context)
        {
            // If there is an image in the view model, copy it to the PaintControl's drawing surface
            if (Vm?.Image != null)
            {
                context.DrawImage(Vm.Image, Bounds);
            }

            // If we are in a dragging operation, draw a dashed rectangle
            // The base color for the rectangle is the color for rectangle that the drag operation will draw
            // The alternative color is either black or white to contrast with the base color
            if (Vm?.Dragging == true) {
                var pen = new Pen(new SolidColorBrush(Color.FromRgb(Vm.Red, Vm.Green, Vm.Blue)));
                context.DrawRectangle(pen, Vm.Marquee.Translate(new Vector(0.5, 0.5)));
                byte altColor = (byte)(255 - Vm.Green);
                pen = new Pen(new SolidColorBrush(Color.FromRgb(altColor, altColor, altColor)), dashStyle: DashStyle.Dash);
                context.DrawRectangle(pen, Vm.Marquee.Translate(new Vector(0.5, 0.5)));
            }
        }


    }
}
