using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicViewLocatorSample.ViewModels
{
    public class SecondPageViewModel : ViewModelBase, IPageNavigation
    {
        public SecondPageViewModel()
        {
            // Listen to changes of MailAddress and Password and update CanNavigateNext accordingly
            this.WhenAnyValue(x => x.MailAddress, x => x.Password)
                .Subscribe(_ => UpdateCanNavigateNext());
        }


        private string? _MailAddress;

        /// <summary>
        /// The E-Mail of the user. This field is required
        /// </summary>
        [Required]
        [EmailAddress]
        public string? MailAddress
        {
            get { return _MailAddress; }
            set { this.RaiseAndSetIfChanged(ref _MailAddress, value); }
        }


        private string? _Password;

        /// <summary>
        /// The password of the user. This field is required.
        /// </summary>
        [Required]
        public string? Password
        {
            get { return _Password; }
            set { this.RaiseAndSetIfChanged(ref _Password, value); }
        }


        private bool _CanNavigateNext;

        // For this page the user can only go to the next page if all fields are valid. So we need a private setter.
        public bool CanNavigateNext
        {
            get { return _CanNavigateNext; }
            private set { this.RaiseAndSetIfChanged(ref _CanNavigateNext, value); }
        }


        // We allow navigate back in any case
        public bool CanNavigatePrevious => true;

        private void UpdateCanNavigateNext()
        {
            // Update CanNavigateNext
            CanNavigateNext = 
                   !string.IsNullOrEmpty(_MailAddress) 
                && _MailAddress.Contains("@")
                && !string.IsNullOrEmpty(_Password);
        }
    }
}
