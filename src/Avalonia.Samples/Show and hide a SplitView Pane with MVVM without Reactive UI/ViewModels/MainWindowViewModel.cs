using System.ComponentModel;

namespace Show_and_hide_a_SplitView_Pane_with_MVVM_without_Reactive_UI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool _isSplitViewPaneOpen;
        public bool IsSplitViewPaneOpen
        {
            get => this._isSplitViewPaneOpen;
            set
            {
                this._isSplitViewPaneOpen = value;
                OnPropertyChanged(nameof(IsSplitViewPaneOpen));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindowViewModel()
        {
            // The default value of a boolean variable is false. Hence if you need the Pane to start closed you can avoid this initialization.
            this._isSplitViewPaneOpen = false;

        }
        public void ChangeSplitViewPaneStatusCommand()
        {
            this.IsSplitViewPaneOpen = !this.IsSplitViewPaneOpen;
        }
    }
}
