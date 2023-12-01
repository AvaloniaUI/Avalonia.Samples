namespace MvvmDialogSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public InteractionViewModel InteractionSample { get; } = new InteractionViewModel();

        public CustomInteractionViewModel CustomInteractionViewModel { get; } = new CustomInteractionViewModel();

    }
}
