using System;
using System.Collections.Generic;
using System.Text;

namespace BasicMvvmSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // Add our SimpleViewModel.
        // Note: We need at least a get-accessor for our Properties.
        public SimpleViewModel SimpleViewModel { get; } = new SimpleViewModel();

        
        // Add our RactiveViewModel
        public ReactiveViewModel ReactiveViewModel { get; } = new ReactiveViewModel();
    }
}
