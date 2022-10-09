using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicViewLocatorSample.ViewModels
{
    /// <summary>
    /// An interface for page navigation.
    /// </summary>
    public interface IPageNavigation
    {
        /// <summary>
        /// Gets if the user can navigate to the next page
        /// </summary>
        bool CanNavigateNext { get; }

        /// <summary>
        /// Gets if the user can navigate to the previous page
        /// </summary>
        bool CanNavigatePrevious { get; }
    }
}
