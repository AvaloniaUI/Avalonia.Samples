using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicViewLocatorSample.ViewModels
{
    /// <summary>
    ///  This is our ViewModel for the first page
    /// </summary>
    public class FirstPageViewModel : PageViewModelBase
    {
        /// <summary>
        /// The Title of this page
        /// </summary>
        public string Title => "Welcome to our Wizard-Sample.";
            
        /// <summary>
        /// The content of this page
        /// </summary>
        public string Message => "Press \"Next\" to register yourself.";

        // This is our first page, so we can navigate to the next page in any case
        public override bool CanNavigateNext 
        { 
            get => true;
            protected set => throw new NotSupportedException(); 
        }

        // You cannot go back from this page
        public override bool CanNavigatePrevious
        {
            get => false;
            protected set => throw new NotSupportedException();
        }
    }
}
