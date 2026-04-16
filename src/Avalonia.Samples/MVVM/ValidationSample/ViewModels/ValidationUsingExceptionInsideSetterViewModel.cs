using ReactiveUI;
using System;

namespace ValidationSample.ViewModels
{
    public class ValidationUsingExceptionInsideSetterViewModel : ViewModelBase
    {

        private string? _EMail;

        /// <summary>
        /// Validation using Exceptions (only inside setter allowed!)
        /// </summary>
        public string? EMail
        {
            get { return _EMail; }
            set 
            {
                /* TIP:
                 Depending on your IDE-Settings the debugger may stop execution when the `Exception` is hit.
                 You can safely click `Resume` or `Mute and Resume` since the exception will be handled by Avalonia. */
                
                // The field may not be null or empty
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(EMail), "This field is required");
                }
                // The field must contain an '@' sign
                else if (!value.Contains('@'))
                {
                    throw new ArgumentException(nameof(EMail), "Not a valid E-Mail-Address");
                }
                // The checks were successful, so we can store the value. 
                else
                { 
                    this.RaiseAndSetIfChanged(ref _EMail, value); 
                } 
            }
        }
    }
}
