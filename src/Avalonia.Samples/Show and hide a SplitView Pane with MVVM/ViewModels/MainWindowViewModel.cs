using ReactiveUI;
using System.Windows.Input;

namespace Show_and_hide_a_SplitView_Pane_with_MVVM.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _isSplitViewPaneOpen;
        public bool IsSplitViewPaneOpen
        {
            get => this._isSplitViewPaneOpen;
            set
            {
                this.RaiseAndSetIfChanged(ref this._isSplitViewPaneOpen, value);
            }
        }


        public MainWindowViewModel()
        {
            // The default value of a boolean variable is false. Hence if you need the Pane to start closed you can avoid this initialization.
            this._isSplitViewPaneOpen = false;
            this.ChangeSplitViewPaneStatusCommand = ReactiveCommand.Create(() =>
            {
                this.IsSplitViewPaneOpen = !this.IsSplitViewPaneOpen;
            });
        }
        public ICommand ChangeSplitViewPaneStatusCommand { get; }
    }
}
