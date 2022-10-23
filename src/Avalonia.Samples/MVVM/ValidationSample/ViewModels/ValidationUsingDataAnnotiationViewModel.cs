using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ValidationSample.ViewModels
{
    public class ValidationUsingDataAnnotiationViewModel : ViewModelBase
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
