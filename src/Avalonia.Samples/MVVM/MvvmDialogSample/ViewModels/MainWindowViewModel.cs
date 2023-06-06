namespace MvvmDialogSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public InteractionSample InteractionSample { get; } = new InteractionSample();

        public DialogServiceViewModel DialogServiceSample { get; } = new DialogServiceViewModel();
    }
}
