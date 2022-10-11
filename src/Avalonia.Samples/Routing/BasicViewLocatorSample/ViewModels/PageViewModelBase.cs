using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicViewLocatorSample.ViewModels
{
    /// <summary>
    /// An abstract class for enabling page navigation.
    /// </summary>
    public abstract class PageViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Gets if the user can navigate to the next page
        /// </summary>
        public abstract bool CanNavigateNext { get; protected set; }

        /// <summary>
        /// Gets if the user can navigate to the previous page
        /// </summary>
        public abstract bool CanNavigatePrevious { get; protected set; }
    }
}
