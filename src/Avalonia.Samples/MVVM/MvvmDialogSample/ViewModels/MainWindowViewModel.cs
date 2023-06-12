namespace MvvmDialogSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public InteractionSample InteractionSample { get; } = new InteractionSample();

        public CustomInteractionViewModel CustomInteractionViewModel { get; } = new CustomInteractionViewModel();

        public DialogServiceViewModel DialogServiceSample { get; } = new DialogServiceViewModel();
    }
}
