using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CommandSample.ViewModels;

/// <summary>
/// This ViewModel will demonstrate how the <see href="https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/">CommunityToolkit-MVVM package</see>
/// can be used to create Commands.
/// </summary>
/// <remarks>
/// If you want to use the SourceGenerators it provides, remember to mark the class as <c>partial</c>.
/// </remarks>
public partial class CommunityToolkitCommandsViewModel : ObservableObject // Note: Since our ViewModelBase isn't an ObservableObject, we have to specify it on our own. This may be different on your side. 
{
    // We can use the constructor to initialize the Commands.
    public CommunityToolkitCommandsViewModel()
    {
        // Init OpenThePodBayDoorsDirectCommand    
        OpenThePodBayDoorsDirectCommand = new RelayCommand(OpenThePodBayDoors);
    }

    /// <summary>
    /// This command will ask HAL-9000 to open the pod bay doors
    /// </summary>
    /// <remarks>
    /// Note: We use the interface ICommand here because this makes things more flexible.
    /// </remarks> 
    public ICommand OpenThePodBayDoorsDirectCommand { get; }

    // The method that will be executed when the command is invoked
    private void OpenThePodBayDoors()
    {
        ConversationLog.Clear();
        AddToConvo("I'm sorry, Dave, I'm afraid I can't do that.");
    }

    /// <summary>
    /// This command will ask HAL to open the pod bay doors, but this time we
    /// check that the command is issued by a fellow robot (really any non-null name)
    /// </summary>
    /// <remarks>
    /// We use the provided <see href="https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/generators/relaycommand">source generator</see>,
    /// since this is quite convenient. We just have to add the <c>[RelayCommand]</c>-attribute.
    /// Please visit the docs for more info about this.
    /// </remarks>
    [RelayCommand(CanExecute = nameof(CanRobotOpenTheDoor))]
    private void OpenThePodBayDoorsFellowRobot(string? robotName)
    {
        ConversationLog.Clear();
        AddToConvo($"Hello {robotName}, the Pod Bay is open :-)");
    }

    /// <summary>
    /// Check if the Robot can open the door (it needs to have a valid name)
    /// </summary>
    /// <returns>true if the robot has a name</returns>
    private bool CanRobotOpenTheDoor() => !string.IsNullOrWhiteSpace(RobotName);

    /// <summary>
    /// The name of a robot. If the name is null or empty, there is no other robot present.
    /// </summary>
    [ObservableProperty] 
    [NotifyCanExecuteChangedFor(nameof(OpenThePodBayDoorsFellowRobotCommand))]
    private string? _robotName;

    /// <summary>
    /// This command will start an async task for the multi-step Pod bay opening sequence
    /// </summary>
    /// <remarks>
    /// This method is an async Task because opening the pod bay doors can take long time.
    /// We don't want our UI to become unresponsive.
    /// </remarks>
    [RelayCommand]
    private async Task OpenThePodBayDoorsAsync()
    {
        ConversationLog.Clear();
        AddToConvo("Preparing to open the Pod Bay...");
        // wait a second
        await Task.Delay(1000);

        AddToConvo("Depressurizing Airlock...");
        // wait 2 seconds
        await Task.Delay(2000);

        AddToConvo("Retracting blast doors...");
        // wait 2 more seconds
        await Task.Delay(2000);

        AddToConvo("Pod Bay is open to space!");
    }

    // Conversation Log (our output)

    /// <summary>
    ///  This collection will store what the computer said
    /// </summary>
    public ObservableCollection<string> ConversationLog { get; } = new ObservableCollection<string>();

    // Just a helper to add content to ConversationLog
    private void AddToConvo(string content)
    {
        ConversationLog.Add(content);
    }
}