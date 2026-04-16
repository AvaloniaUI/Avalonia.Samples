using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommandSample.ViewModels
{
    public class ReactiveUiCommandsViewModel : ViewModelBase
    {
        // We use the constructor to initialize the Commands.
        public ReactiveUiCommandsViewModel()
        {
            // We initiate our Commands using ReactiveCommand.Create...
            // see: https://www.reactiveui.net/docs/handbook/commands/
            
            // Init OpenThePodBayDoorsDirectCommand    
            OpenThePodBayDoorsDirectCommand = ReactiveCommand.Create(OpenThePodBayDoors);
            
            // Init OpenThePodBayDoorsFellowRobotCommand
            // The IObservable<bool> is needed to enable or disable the command depending on valid parameters
            // The Observable listens to RobotName and will enable the Command if the name is not empty.
            IObservable<bool> canExecuteFellowRobotCommand =
                this.WhenAnyValue(vm => vm.RobotName, (name) => !string.IsNullOrEmpty(name));

            OpenThePodBayDoorsFellowRobotCommand = 
                ReactiveCommand.Create<string?>(name => OpenThePodBayDoorsFellowRobot(name), canExecuteFellowRobotCommand);

            // Init OpenThePodBayDoorsAsyncCommand
            OpenThePodBayDoorsAsyncCommand = ReactiveCommand.CreateFromTask(OpenThePodBayDoorsAsync);
        }
        
        /// <summary>
        /// This command will ask HAL-9000 to open the pod bay doors
        /// </summary>
        /// // Note: We use the interface ICommand here because this makes things more flexible.
        public ICommand OpenThePodBayDoorsDirectCommand { get; }
        
        // The method that will be executed when the command is invoked
        private void OpenThePodBayDoors()
        {    
            ConversationLog.Clear();
            AddToConvo( "I'm sorry, Dave, I'm afraid I can't do that.");
        }
        
        /// <summary>
        /// This command will ask HAL to open the pod bay doors, but this time we
        /// check that the command is issued by a fellow robot (really any non-null name)
        /// </summary>
        public ICommand OpenThePodBayDoorsFellowRobotCommand { get; }

        // The method that will be executed when the command is invoked
        private void OpenThePodBayDoorsFellowRobot(string? robotName)
        {
            ConversationLog.Clear();    
            AddToConvo( $"Hello {robotName}, the Pod Bay is open :-)");
        }

        // Backing field for RobotName
        private string? _RobotName;
        
        /// <summary>
        /// The name of a robot. If the name is null or empty, there is no other robot present.
        /// </summary>
        public string? RobotName
        {
            get => _RobotName;
            set => this.RaiseAndSetIfChanged(ref _RobotName, value);
        }

        /// <summary>
        /// This command will start an async task for the multi-step Pod bay opening sequence
        /// </summary>
        public ICommand OpenThePodBayDoorsAsyncCommand { get; }

        // This method is an async Task because opening the pod bay doors can take long time.
        // We don't want our UI to become unresponsive.
        private async Task OpenThePodBayDoorsAsync()
        {
            ConversationLog.Clear();
            AddToConvo( "Preparing to open the Pod Bay...");
            // wait a second
            await Task.Delay(1000);
	
            AddToConvo( "Depressurizing Airlock...");
            // wait 2 seconds
            await Task.Delay(2000);
	
            AddToConvo( "Retracting blast doors...");
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
}
