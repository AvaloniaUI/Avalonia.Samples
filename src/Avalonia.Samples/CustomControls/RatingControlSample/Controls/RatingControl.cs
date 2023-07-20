using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using System.Collections.Generic;
using System.Linq;

namespace RatingControlSample.Controls
{
    /// <summary>
    /// This control lets the user set a rating, to show their degree of satisfaction 
    /// </summary>

    // This Attribute specifies that "PART_StarsPresenter" is a control, which must be present in the Control-Template
    [TemplatePart("PART_StarsPresenter", typeof(ItemsControl))]
    public class RatingControl : TemplatedControl 
    {
        // this field holds a reference to the part in the control template that holds the rating stars
        private ItemsControl? _starsPresenter;
     
        public RatingControl() 
        { 
            // When a new instance of the control is created, we need to update the rating stars visible
            UpdateStars();
        }

        /// <summary>
        /// Defines the <see cref="NumberOfStars"/> property.
        /// </summary>
        /// <remarks>
        /// We define this property as a styled property, so you can set this property inside your style of the rating control. 
        /// </remarks>
        public static readonly StyledProperty<int> NumberOfStarsProperty =
            AvaloniaProperty.Register<RatingControl, int>(
                nameof(NumberOfStars),          // Sets the name of the property
                defaultValue: 5,                // The default value of this property
                coerce: CoerceNumberOfStars);   // Ensures that we always have a valid number of stars


        /// <summary>
        /// Gets or sets the number of available stars
        /// </summary>
        public int NumberOfStars
        {
            get { return GetValue(NumberOfStarsProperty); }
            set { SetValue(NumberOfStarsProperty, value); }
        }

        /// <summary>
        /// This function will coerce the <see cref="NumberOfStars"/> property. The minimum allowed number is 1
        /// </summary>
        /// <param name="sender">the RatingControl-instance calling this method</param>
        /// <param name="value">the value to coerce</param>
        /// <returns>The coerced value</returns>
        private static int CoerceNumberOfStars(AvaloniaObject sender, int value)
        {
            // the value should not be lower than 1.
            // Hint: You can also return Math.Max(1, value)
            if (value < 1)
            {
                return 1;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Defines the <see cref="Value"/> property.
        /// </summary>
        /// <remarks>
        /// This property doesn't need to be styled. Therefore we can use a direct property, which improves performance and 
        /// allows us to add validation support.
        /// </remarks>
        public static readonly DirectProperty<RatingControl, int> ValueProperty =
            AvaloniaProperty.RegisterDirect<RatingControl, int>(
                nameof(Value),                            // The name of the property     
                o => o.Value,                  // The getter of the property
                (o, v) => o.Value = v,      // The setter of the property
                defaultBindingMode: BindingMode.TwoWay,   // We change the default binding mode to be two-way, so if the user selects a new value, it will automatically update the bound property
                enableDataValidation: true);              // Enables DataValidation

        // For direct properties we need to have a backing field
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
        /// Defines the <see cref="Stars"/> property.
        /// </summary>
        /// <remarks>
        /// ´This property holds a read-only array of stars. 
        /// </remarks>
        public static readonly DirectProperty<RatingControl, IEnumerable<int>> StarsProperty =
            AvaloniaProperty.RegisterDirect < RatingControl, IEnumerable<int>>(
                nameof(Stars),              // The name of the Property
                o => o.Stars);   // The getter. As we don't add a setter, this property is read-only

        // For read-only properties we need to have a backing field. The default value is [1..5]
        private IEnumerable<int> _stars = Enumerable.Range(1, 5);

        /// <summary>
        /// Gets the current collection of visible stars
        /// </summary>
        public IEnumerable<int> Stars
        {
            get { return _stars; }
            private set { SetAndRaise(StarsProperty, ref _stars, value); } // make sure the setter is private
        }

        // called when the number of stars changed
        private void UpdateStars()
        {
            // Stars is an array from 1 to NumberOfStars
            Stars = Enumerable.Range(1, NumberOfStars);
        }

        // We override OnPropertyChanged of the base class. That way we can react on property changes
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            // if the changed property is the NumberOfStarsProperty, we need to update the stars
            if (change.Property == NumberOfStarsProperty) 
            {
                UpdateStars();
            }
        }

        // We override what happens when the control template is being applied. 
        // That way we can for example listen to events of controls which are part of the template
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            // if we had a control template before, we need to unsubscribe any event listeners
            if(_starsPresenter is not null)
            {
                _starsPresenter.PointerReleased-= StarsPresenter_PointerReleased;
            }

            // try to find the control with the given name
            _starsPresenter = e.NameScope.Find("PART_StarsPresenter") as ItemsControl;

            // listen to pointer-released events on the stars presenter.
            if(_starsPresenter != null)
            {
                _starsPresenter.PointerReleased += StarsPresenter_PointerReleased;
            }
        }

        /// <summary>
        /// Called to update the validation state for properties for which data validation is
        /// enabled.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="state">The current data binding state.</param>
        /// <param name="error">The Exception that was passed</param>
        protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
        {
            base.UpdateDataValidation(property, state, error);
            
            if(property == ValueProperty)
            {
                DataValidationErrors.SetError(this, error);
            }
        }

        private void StarsPresenter_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            // e.Source is the original source of this event. In our case, if the user clicked on a star, the original source is a Path.
            if (e.Source is Path star)
            {
                // The DataContext of the star is one of the numbers we have in the Stars-Collection. 
                // Let's cast the DataContext to an int. If that cast fails, use "0" as a fallback.
                Value = star.DataContext as int? ?? 0;
            }
        }
    }
}
