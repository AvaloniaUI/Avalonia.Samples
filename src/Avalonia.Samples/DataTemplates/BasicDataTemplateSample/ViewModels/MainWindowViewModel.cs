using BasicDataTemplateSample.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicDataTemplateSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// As this is a list of Persons, we can add Students and Teachers here. 
        /// </summary>
        public List<Person> People { get; } = new List<Person>()
        {
            new Teacher
            {
                FirstName = "Mr.",
                LastName = "X",
                Age = 55,
                Sex=Sex.Divers,
                Subject = "My Favorite Subject"
            },
            new Student
            {
                FirstName = "Hello",
                LastName = "World",
                Age = 17,
                Sex= Sex.Male,
                Grade=12
            },
            new Student
            {
                FirstName = "Hello",
                LastName = "Kitty",
                Age = 12,
                Sex= Sex.Female,
                Grade=6
            }
        };
    }
}
