using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RatingControlSample.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets or sets the number of stars. The default value is 6. 
        /// </summary>
        [ObservableProperty]
        public partial int NumberOfStars { get; set; } = 6;
        
        /// <summary>
        /// Gets or sets the current rating value.
        /// The initial value is 2
        /// It must be between 0 and 5.
        /// </summary>
        [ObservableProperty]
        [NotifyDataErrorInfo, Range(0, 5)]
        public partial int RatingValue { get; set; } = 2;
    }
}
