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
        public MainWindowViewModel()
        {
            // Create our Commands using ReactiveCommand.Create...
            // see: https://www.reactiveui.net/docs/handbook/commands/

            BringMeACoffeeCommand = ReactiveCommand.Create(() => BringMeACoffee());

            var canExecuteBringMyFriendABeerCommand =
                this.WhenAnyValue(vm => vm.FriendsName, (name) => !string.IsNullOrEmpty(name));

            BringMyFriendACoffeeCommand = 
                ReactiveCommand.Create<string?>(name => BringMyFriendACoffee(name), canExecuteBringMyFriendABeerCommand);

            BakeUsACakeCommand = ReactiveCommand.CreateFromTask(() => BakeUsACakeAsync());
        }

        // Bring me a beer

        /// <summary>
        /// This command will ask our wife to bring you a beer.
        /// </summary>
        // Note: We use the interface ICommand here because this makes things more flexible. 
        public ICommand BringMeACoffeeCommand { get; }

        private void BringMeACoffee()
        {
            WhatYourWifeSaid.Clear();
            WifeSays("Go, get your self a coffee !!");
        }


        // Bring my Friend a beer

        private string? _FriendsName;

        /// <summary>
        /// Enter the name of your friend here. If the name is null or empty, you have no friend to bring a beer.
        /// </summary>
        public string? FriendsName
        {
            get => _FriendsName;
            set => this.RaiseAndSetIfChanged(ref _FriendsName, value);
        }

        /// <summary>
        /// This command will ask our wife to bring your a beer.
        /// </summary>
        // Note: We use the interface ICommand here because this makes things more flexible. 
        public ICommand BringMyFriendACoffeeCommand { get; }

        private void BringMyFriendACoffee(string? friendsName)
        {
            WhatYourWifeSaid.Clear();
            WifeSays($"Dear {friendsName}, here is your coffee :-)");
        }


        // Bake us a Cake

        /// <summary>
        /// This command will ask our wife to bake us a cake.
        /// </summary>
        public ICommand BakeUsACakeCommand { get; }

        // This method is an async Task because baking a cake can take long time. We don't want to our UI to be unresponsive.
        private async Task BakeUsACakeAsync()
        {
            WhatYourWifeSaid.Clear();
            WifeSays("Sure, I'll make you an apple pie.");
            // wait a second
            await Task.Delay(1000);

            WifeSays("I'm mixing all the integredients.");
            await Task.Delay(2000);

            WifeSays("It's in the oven. Wait another 2 seconds.");
            await Task.Delay(2000);

            WifeSays("Here is your cake, fresh from the oven.");
        }

        // Your wife (our output)

        /// <summary>
        ///  This will store what our wife said
        /// </summary>
        public ObservableCollection<string> WhatYourWifeSaid { get; } = new ObservableCollection<string>();

        private void WifeSays(string content)
        {
            WhatYourWifeSaid.Add(content);
        }
    }
}
