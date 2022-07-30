using System;
using System.Collections.Generic;
using System.Text;

namespace BasicMvvmSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // Add a SimpleViewModel
        public SimpleViewModel SimpleViewModel => new SimpleViewModel();
    }
}
