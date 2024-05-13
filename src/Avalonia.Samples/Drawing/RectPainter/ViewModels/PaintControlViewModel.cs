using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System.ComponentModel;

namespace RectPainter.ViewModels
{
    public class PaintControlViewModel: INotifyPropertyChanged
    {
        private bool _dragging = false;
        private Point _origin = new Point(0, 0);
        private Point _pos = new Point(0, 0);
        private Rect _marquee = new Rect();
        private RenderTargetBitmap? _image = null;

        // Are we in a drag operation?
        public bool Dragging
        {
            get => _dragging;
            set
            {
                if (_dragging != value)
                {
                    _dragging = value;
                    OnPropertyChanged(nameof(Dragging));
                }
            }
        }

        // The current mouse position
        public Point Pos
        {
            get => _pos;
            set
            {
                if (_pos != value)
                {
                    _pos = value;
                    OnPropertyChanged(nameof(Pos));
                }
            }
        }

        // The mouse position at the start of the drag operation
        public Point Origin
        {
            get => _origin;
            set
            {
                if (_origin != value)
                {
                    _origin = value;
                    OnPropertyChanged(nameof(Origin));
                }
            }
        }

        // The drag rectangle
        public Rect Marquee
        {
            get => _marquee;
            set
            {
                if (_marquee != value)
                {
                    _marquee = value;
                    OnPropertyChanged(nameof(Marquee));
                }
            }
        }

        // The color for the rectangle to be drawn
        // This can be controlled by pressing Shift, Ctrl and Alt keys
        public byte Red { get; set; } = 0;
        public byte Green { get; set; } = 0;
        public byte Blue { get; set; } = 0;

        // The bitmap full of rectangles
        public IImage? Image
        {
            get => _image;
        }

        // When the control changes, we need to change the bitmap to match
        public void SetImageSize(PixelSize size)
        {
            RenderTargetBitmap newImage = new RenderTargetBitmap(new PixelSize(size.Width, size.Height), new Vector(96, 96));
            if (_image != null)
            {
                // If there was already a bitmap, copy it into the new one.
                using (var context = newImage.CreateDrawingContext())
                {
                    context.DrawImage(_image, new Rect(0, 0, size.Width, size.Height), new Rect(0, 0, size.Width, size.Height));
                }
            }
            _image = newImage;
            OnPropertyChanged(nameof(Image));
        }

        public void AddRectangle()
        {
            if (_image != null)
            {
                // Create a new image, copy the old one, and then add a new rectangle to it
                // using the current marquee and color
                RenderTargetBitmap newImage = new RenderTargetBitmap(_image.PixelSize, _image.Dpi);
                using (var context = newImage.CreateDrawingContext())
                {
                    context.DrawImage(_image, new Rect(0, 0, _image.PixelSize.Width, _image.PixelSize.Height));
                    var brush = new SolidColorBrush(Color.FromRgb(Red, Green, Blue));
                    context.FillRectangle(brush, _marquee);
                }
                _image.Dispose();
                _image = newImage;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
