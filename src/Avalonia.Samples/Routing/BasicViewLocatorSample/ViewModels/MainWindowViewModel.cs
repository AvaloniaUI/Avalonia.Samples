using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace BasicViewLocatorSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            // Set current page to first on start up
            _CurrentPage = Pages[0];

            var canNavNext = this.WhenAnyValue(x => x.CurrentPage.CanNavigateNext);
            var canNavPrev = this.WhenAnyValue(x => x.CurrentPage.CanNavigatePrevious);

            NavigateNextCommand = ReactiveCommand.Create(NavigateNext, canNavNext);
            NavigatePreviousCommand = ReactiveCommand.Create(NavigatePrevious, canNavPrev);
        }

        // A read.only array of possible pages
        private readonly IPageNavigation[] Pages = 
        { 
            new FirstPageViewModel(),
            new SecondPageViewModel(),
            new ThirdPageViewModel()
        };

        // the current index of the page
        private int _CurrentPageIndex;

        // The default is the first page
        private IPageNavigation _CurrentPage;

        /// <summary>
        /// Gets the current page. The property is read-only
        /// </summary>
        public IPageNavigation CurrentPage
        {
            get { return _CurrentPage; }
            private set { this.RaiseAndSetIfChanged(ref _CurrentPage, value); }
        }

        public ICommand NavigateNextCommand { get; }

        private void NavigateNext()
        {
            // increment current index
            _CurrentPageIndex++;
            CurrentPage = Pages[_CurrentPageIndex];
        }

        public ICommand NavigatePreviousCommand { get; }
        private void NavigatePrevious()
        {
            // decrement current index
            _CurrentPageIndex--;
            CurrentPage = Pages[_CurrentPageIndex];
        }
    }
}
