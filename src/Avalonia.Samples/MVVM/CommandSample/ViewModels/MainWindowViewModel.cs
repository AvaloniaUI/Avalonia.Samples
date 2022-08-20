using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommandSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // We use the constructor to initialize the Commands.
        public MainWindowViewModel()
        {
            // We create our Commands using ReactiveCommand.Create...
            // see: https://www.reactiveui.net/docs/handbook/commands/

            // Init BringMeACoffeeCommand
            BringMeACoffeeCommand = ReactiveCommand.Create(BringMeACoffee);

            // Init BringMyFriendACoffeeCommand
            // The IObservable<bool> is needed to enable or disable the command depending on valid parameters
            // The Observable listens to FriendsName and will enable the Command if the name is not empty.
            IObservable<bool> canExecuteBringMyFriendABeerCommand =
                this.WhenAnyValue(vm => vm.FriendsName, (name) => !string.IsNullOrEmpty(name));

            BringMyFriendACoffeeCommand =
                ReactiveCommand.Create<string?>(name => BringMyFriendACoffee(name), canExecuteBringMyFriendABeerCommand);

            // Init BakeUsACakeCommand
            BakeUsACakeCommand = ReactiveCommand.CreateFromTask(BakeUsACakeAsync);
        }


        /// <summary>
        /// This command will ask our wife to bring us a coffee.
        /// </summary>
        // Note: We use the interface ICommand here because this makes things more flexible. 
        public ICommand BringMeACoffeeCommand { get; }

        // The method that will be executed when the command is invoked
        private void BringMeACoffee()
        {
            WhatYourWifeSaid.Clear();
            WifeSays("Go, get your self a coffee !!");
        }


        // Backing field for FriendsName
        private string? _FriendsName;

        /// <summary>
        /// Enter the name of your friend here. If the name is null or empty, you have no friend to bring a coffee.
        /// </summary>
        public string? FriendsName
        {
            get => _FriendsName;
            set => this.RaiseAndSetIfChanged(ref _FriendsName, value);
        }

        /// <summary>
        /// This command will ask our wife to bring your friend a beer.
        /// </summary>
        // Note: We use the interface ICommand here because this makes things more flexible. 
        public ICommand BringMyFriendACoffeeCommand { get; }

        // The method that will be executed when the command is invoked
        private void BringMyFriendACoffee(string? friendsName)
        {
            WhatYourWifeSaid.Clear();
            WifeSays($"Dear {friendsName}, here is your coffee :-)");
        }


        /// <summary>
        /// This command will ask our wife to bake us a cake.
        /// </summary>
        public ICommand BakeUsACakeCommand { get; }

        // This method is an async Task because baking a cake can take long time.
        // We don't want our UI to become unresponsive.
        private async Task BakeUsACakeAsync()
        {
            WhatYourWifeSaid.Clear();
            WifeSays("Sure, I'll make you an apple pie.");
            // wait a second
            await Task.Delay(1000);

            WifeSays("I'm mixing all the ingredients.");
            await Task.Delay(2000);

            WifeSays("It's in the oven. Wait another 2 seconds.");
            await Task.Delay(2000);

            WifeSays("Here is your cake, fresh from the oven.");
        }

        // Your wife (our output)

        /// <summary>
        ///  This collection will store what our wife said
        /// </summary>
        public ObservableCollection<string> WhatYourWifeSaid { get; } = new ObservableCollection<string>();

        // Just a helper to add content to WhatYourWifeSaid
        private void WifeSays(string content)
        {
            WhatYourWifeSaid.Add(content);
        }
    }
}
