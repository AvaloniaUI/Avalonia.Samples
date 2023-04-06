namespace BasicDataTemplateSample.Models
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
        /// Gets or sets the age of the person. You can only set this property on init. 
        /// </summary>
        public int Age { get; init; }

        /// <summary>
        /// Gets or sets the sex of the person. You can only set this property on init. 
        /// </summary>
        public Sex Sex { get; init; }

        // The default DataTemplate will show whatever ToString() will provide. So as a fist idea let's change what ToString() will provide.
        public override string ToString()
        {
            return $"{FirstName} {LastName} (Age: {Age}, Sex: {Sex})";
        }
    }
}
