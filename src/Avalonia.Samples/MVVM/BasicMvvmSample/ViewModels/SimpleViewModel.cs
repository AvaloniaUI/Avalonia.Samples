using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BasicMvvmSample.ViewModels
{
    // This is our simple ViewModel. We need to implement the interface "INotifyPropertyChanged"
    // in order to notify the user interface if any of our properties changed.
    public class SimpleViewModel : INotifyPropertyChanged
    {
        // This event is implemented by "INotifyPropertyChanged" and all we need to inform 
        // our view about changes.
        public event PropertyChangedEventHandler? PropertyChanged;

        // For convinience we add a method which will raise the above event.
        private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // ---- Add some Properties ----

        private string? _Name; // This is our backing field for Name

        public string? Name
        {
            get 
            {
                return _Name; 
            }
            set
            {
                // We only want to update the UI if the Name actually changed
                if (_Name != value)
                {
                    // 1. update our backing field
                    _Name = value;

                    // 2. We call RaisePropertyChanged() to notify the UI about changes. 
                    // We can omit the property name here because [CallerMemberName] will provide it for us.  
                    RaisePropertyChanged();

                    // 3. Greeting also changed. So let's notify the UI about it. 
                    RaisePropertyChanged(nameof(Greeting));
                }
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
