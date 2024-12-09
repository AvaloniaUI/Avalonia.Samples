using System;
using System.Collections.Generic;

namespace MvvmDialogSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets a list of available tabs
        /// </summary>
        public object[] Samples { get; } = 
        {
            new InteractionViewModel(),
            new CustomInteractionViewModel()
        };
    }
}
