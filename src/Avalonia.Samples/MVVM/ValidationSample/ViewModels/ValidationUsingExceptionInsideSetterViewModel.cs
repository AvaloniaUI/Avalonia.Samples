using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
