using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncDataTemplateSample.Models
{
    public class Person
    {
        /// <summary>
        /// Gets or sets the first name of the person. You can only set this property on init. 
        /// </summary>
        public string? FirstName { get; init; }

        /// <summary>
        /// Gets or sets the last name of the person. You can only set this property on init. 
        /// </summary>
        public string? LastName { get; init; }

        /// <summary>
        /// Gets or sets the sex of the person. You can only set this property on init. 
        /// </summary>
        public Sex Sex { get; init; }

        // Override ToString()
        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
