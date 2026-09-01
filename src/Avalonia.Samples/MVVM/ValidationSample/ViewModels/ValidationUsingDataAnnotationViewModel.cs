using ReactiveUI;
using System.ComponentModel.DataAnnotations;
using ValidationSample.ViewModels;

namespace ValidationSample.ViewModels
{
    public class ValidationUsingDataAnnotationViewModel : ViewModelBase
    {
        private string? _EMail;

        /// <summary>
        /// Validation using DataAnnotation
        /// </summary>
        [Required]
        [EmailAddress]
        public string? EMail
        {
            get { return _EMail; }
            set { this.RaiseAndSetIfChanged(ref _EMail, value); }
        }
    }
}
