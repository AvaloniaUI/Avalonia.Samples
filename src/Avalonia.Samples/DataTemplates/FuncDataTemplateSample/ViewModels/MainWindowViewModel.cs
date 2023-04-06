using FuncDataTemplateSample.Models;
using System.Collections.Generic;

namespace FuncDataTemplateSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public List<Person> People { get; } = new List<Person>()
        {
            new Person
            {
                FirstName = "Mr.",
                LastName = "X",
                Sex=Sex.Diverse
            },
            new Person
            {
                FirstName = "Hello",
                LastName = "World",
                Sex= Sex.Male
            },
            new Person
            {
                FirstName = "Hello",
                LastName = "Kitty",
                Sex= Sex.Female
            }
        };
    }
}
