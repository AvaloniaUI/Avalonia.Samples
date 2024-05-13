using System.ComponentModel;

namespace RectPainter.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _mousePosition = string.Empty;
        private string _rect = string.Empty;
        private PaintControlViewModel? _vm = null;

        // A text rendering of the current mouse position
        public string MousePosition
        {
            get => _mousePosition;
            set
            {
                _mousePosition = value;
                OnPropertyChanged(nameof(MousePosition));
            }
        }

        // A text rendering of the current marquee dimensions
        public string Rect
        {
            get => _rect;
            set
            {
                _rect = value;
                OnPropertyChanged(nameof(Rect));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // The view model for the PaintControl, is a child of this view model
        // just as the PaintControl itself is a child of the MainWindow
        public PaintControlViewModel? Vm { 
            get => _vm;
            set
            {
                if (_vm != value)
                {
                    // Remove event handlers from any previous view model
                    if (_vm != null)
                        _vm.PropertyChanged -= _vm_PropertyChanged;

                    _vm = value;

                    // Add event handlers for this view model
                    if (_vm != null)
                        _vm.PropertyChanged += _vm_PropertyChanged;
                }
            }
        }

        private void _vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(PaintControlViewModel.Dragging): 
                    {
                        // Update the text for Position and Marquee
                        SetPos();
                        SetRect();
                        break;
                    }

                case nameof(PaintControlViewModel.Pos):
                    {
                        // Update the text for Position
                        SetPos();
                        break;
                    }

                case nameof(PaintControlViewModel.Marquee):
                    {
                        // Update the text for the Marquee
                        SetRect();
                        break;
                    }

            }
        }

        private void SetPos()
        {
            MousePosition = $"{Vm?.Pos.X} {Vm?.Pos.Y}";
        }

        private void SetRect()
        {
            if (Vm?.Dragging == true)
            {
                Rect = $"{Vm.Marquee.Left} {Vm.Marquee.Top}  {Vm.Marquee.Width} {Vm.Marquee.Height}";
            }
            else
            {
                Rect = string.Empty;
            }
        }
    }
}
