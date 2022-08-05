using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicMvvmSample.ViewModels
{
    // Instead of implementing "INotifyPropertyChanged" on our own we use "ReachtiveObject" as 
    // our base class. Read more about it here: https://www.reactiveui.net
    public class ReactiveViewModel : ReactiveObject
    {
        public ReactiveViewModel()
        {
            // We can listen to any property changes with "WhenAnyValue" and do whatever we want in "Subscribe".
            this.WhenAnyValue(o => o.Name)
                .Subscribe(o => this.RaisePropertyChanged(nameof(Greeting)));
        }

        private string? _Name; // This is our backing field for Name

        public string? Name
        {
            get
            {
                return _Name;
            }
            set
            {
                // We can use "RaiseAndSetIfChanged" to check if the value changed and automatically notify the UI
                this.RaiseAndSetIfChanged(ref _Name, value);
            }
        }

        // Greeting will change based on a Name.
        public string Greeting
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                {
                    // If no Name is provided, use a default Greeting
                    return "Hello World from Avalonia.Samples";
                }
                else
                {
                    // else Greet the User.
                    return $"Hello {Name}";
                }
            }
        }
    }
}
