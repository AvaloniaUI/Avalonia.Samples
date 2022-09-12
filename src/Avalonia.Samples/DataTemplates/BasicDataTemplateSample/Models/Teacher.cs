using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicDataTemplateSample.Models
{
    public class Teacher : Person
    {
        /// <summary>
        /// Gets or sets the subject that the teacher is teaching. You can only set this property on init.
        /// </summary>
        public string? Subject { get; init; }
    }
}
