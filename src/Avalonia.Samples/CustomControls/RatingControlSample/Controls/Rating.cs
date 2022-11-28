using Avalonia;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatingControlSample.Controls
{
    public class Rating : TemplatedControl
    {
        /// <summary>
        /// Defines the <see cref="Minimum"/> property.
        /// </summary>
        public static readonly StyledProperty<int> MinimumProperty =
            AvaloniaProperty.Register<Rating, int>(nameof(Minimum), 0);

        /// <summary>
        /// Gets or sets the minimum allowed value
        /// </summary>
        public int Minimum
        {
            get { return GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }


        /// <summary>
        /// Defines the <see cref="Maximum"/> property.
        /// </summary>
        public static readonly StyledProperty<int> MaximumProperty =
            AvaloniaProperty.Register<Rating, int>(nameof(Maximum), 5);

        /// <summary>
        /// Gets or sets the maximum allowed value
        /// </summary>
        public int Maximum
        {
            get { return GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }


        /// <summary>
        /// Defines the <see cref="Value"/> property.
        /// </summary>
        public static readonly DirectProperty<Rating, int> ValueProperty =
            AvaloniaProperty.RegisterDirect<Rating, int>(
                nameof(Value),
                o => o.Value,
                (o, v) => o.Value = v);

        /// <summary>
        /// Gets or sets the current value
        /// </summary>
        public int Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public IEnumerable<int> Stars => Enumerable.Range(Minimum, Maximum - Minimum);
    }
}
