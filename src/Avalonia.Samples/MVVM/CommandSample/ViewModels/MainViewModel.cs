namespace CommandSample.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ReactiveUiCommandsViewModel ReactiveUiCommandsViewModel { get; } = new ReactiveUiCommandsViewModel();
    
    public CommunityToolkitCommandsViewModel CommunityToolkitCommandsViewModel { get; } = new CommunityToolkitCommandsViewModel();
}