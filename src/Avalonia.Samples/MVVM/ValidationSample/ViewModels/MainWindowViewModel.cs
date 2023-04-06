namespace ValidationSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets a ViewModel showing how to use DataAnnotations for validation
        /// </summary>
        public ValidationUsingDataAnnotationViewModel ValidationUsingDataAnnotationViewModel { get; } 
            = new ValidationUsingDataAnnotationViewModel();

        /// <summary>
        /// Gets a ViewModel showing how to use INotifyDataErrorInfo for validation
        /// </summary>
        public ValidationUsingINotifyDataErrorInfoViewModel ValidationUsingINotifyDataErrorInfoViewModel { get; } 
            = new ValidationUsingINotifyDataErrorInfoViewModel();

        /// <summary>
        /// Gets a ViewModel showing how to use Exceptions inside the setter for validation
        /// </summary>
        public ValidationUsingExceptionInsideSetterViewModel ValidationUsingExceptionInsideSetterViewModel { get; } 
            = new ValidationUsingExceptionInsideSetterViewModel();
    }
}
