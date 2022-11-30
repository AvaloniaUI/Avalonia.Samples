using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatingControlSample.Controls
{
    [TemplatePart("PART_StarsPresenter", typeof(ItemsControl))]
    public class RatingControl : TemplatedControl
    {
        private ItemsControl? _starsPresenter;

        public RatingControl() 
        { 
            UpdateStars();
        }

        /// <summary>
        /// Defines the <see cref="NumberOfStars"/> property.
        /// </summary>
        public static readonly StyledProperty<int> NumberOfStarsProperty =
            AvaloniaProperty.Register<RatingControl, int>(nameof(NumberOfStars), 5);

        /// <summary>
        /// Gets or sets the number of available stars
        /// </summary>
        public int NumberOfStars
        {
            get { return GetValue(NumberOfStarsProperty); }
            set { SetValue(NumberOfStarsProperty, value); }
        }


        /// <summary>
        /// Defines the <see cref="Value"/> property.
        /// </summary>
        public static readonly DirectProperty<RatingControl, int> ValueProperty =
            AvaloniaProperty.RegisterDirect<RatingControl, int>(
                nameof(Value),
                o => o.Value,
                (o, v) => o.Value = v,
                defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        private int _value;

        /// <summary>
        /// Gets or sets the current value
        /// </summary>
        public int Value
        {
            get { return _value; }
            set { SetAndRaise(ValueProperty, ref _value, value); }
        }


        /// <summary>
        /// Defines the <see cref="Value"/> property.
        /// </summary>
        public static readonly DirectProperty<RatingControl, IEnumerable<int>> StarsProperty =
            AvaloniaProperty.RegisterDirect < RatingControl, IEnumerable<int>>(
                nameof(Stars),
                o => o.Stars);

        private IEnumerable<int> _stars = Enumerable.Range(1, 5);

        /// <summary>
        /// Gets the current collection of visible stars
        /// </summary>
        public IEnumerable<int> Stars
        {
            get { return _stars; }
            private set { SetAndRaise(StarsProperty, ref _stars, value); }
        }

        private void UpdateStars()
        {
            Stars = Enumerable.Range(1, NumberOfStars);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            switch (change.Property.Name)
            {
                case nameof(NumberOfStars):
                    UpdateStars();
                    break;
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            if(_starsPresenter is not null)
            {
                _starsPresenter.PointerReleased-= StarsPresenter_PointerReleased;
            }

            _starsPresenter = e.NameScope.Find("PART_StarsPresenter") as ItemsControl;

            if(_starsPresenter != null)
            {
                _starsPresenter.PointerReleased += StarsPresenter_PointerReleased; ;
            }
        }

        private void StarsPresenter_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            if (e.Source is Path star)
            {
                Value = star.DataContext as int? ?? 0;
            }
        }
    }
}
