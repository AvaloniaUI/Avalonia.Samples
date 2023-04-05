namespace BasicDataTemplateSample.Models
{
    public class Student : Person
    {
        /// <summary>
        /// Gets or sets the grade of the student. You can only set this property on init. 
        /// </summary>
        public int Grade { get; init; }
    }
}
