using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicViewLocatorSample.ViewModels
{
    public class ThirdPageViewModel : ViewModelBase, IPageNavigation
    {
        // This is the last page, so we cannot navigate next in our sample. 
        public bool CanNavigateNext => false;

        // We navigate back form this page in any case
        public bool CanNavigatePrevious => true;

        // The message to display
        public string Message => "Done";
    }
}
