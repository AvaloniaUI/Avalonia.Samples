using ReactiveUI;

namespace ValueConversionSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // The initial value is 2. 
        private decimal? _Number1 = 2;

        /// <summary>
        /// This is our Number 1
        /// </summary>
        public decimal? Number1
        {
            get { return _Number1; }
            set { this.RaiseAndSetIfChanged(ref _Number1, value); }
        }


        // The initial value is 3.
        private decimal? _Number2 = 3;

        /// <summary>
        /// This is our Number 2
        /// </summary>
        public decimal? Number2
        {
            get { return _Number2; }
            set { this.RaiseAndSetIfChanged(ref _Number2, value); }
        }


        // The initial value is "+" (Add).
        private string _Operator = "+";

        /// <summary>
        /// Gets or sets the operator to use. The initial value is "+"
        /// </summary>
        public string Operator
        {
            get { return _Operator; }
            set { this.RaiseAndSetIfChanged(ref _Operator, value); }
        }

        /// <summary>
        /// Gets a collection of available operators
        /// </summary>
        public string[] AvailableMathOperators { get; } = new string[]
        {
            "+", "-", "*", "/"
        };
    }
}
