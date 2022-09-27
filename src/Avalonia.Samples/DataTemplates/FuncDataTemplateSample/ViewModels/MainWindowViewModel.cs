using FuncDataTemplateSample.Models;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;

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
