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
                // Only values >0 are allowed
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(EMail), "This field is required");
                }
                else if (!value.Contains('@'))
                {
                    throw new ArgumentException(nameof(EMail), "Not a valid E-Mail-Address");
                }
                else
                { 
                    this.RaiseAndSetIfChanged(ref _EMail, value); 
                } 
            }
        }
    }
}
