using Avalonia.Controls.Templates;
using Avalonia.Controls;
using Avalonia.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDataTemplateSample.Models;

namespace IDataTemplateSample.DataTemplates
{
    public class ShapesTemplateSelector : IDataTemplate
    {
        // This Dictionary should store our shapes. We mark this as [Content], so we can directly add elements to it later.
        [Content]
        public Dictionary<string, IDataTemplate> AvailableTemplates { get; } = new Dictionary<string, IDataTemplate>();

        // Build the DataTemplate here
        public IControl Build(object param)
        {
            var key = param.ToString(); // Our Keys in the dictionary are strings, so we call .ToString() to get the key to look up
            if (key is null) // If the key is null, we throw an ArgumentNullException
            {
                throw new ArgumentNullException(nameof(param));
            }
            return AvailableTemplates[key].Build(param); // finally we look up the provided key and let the System build the DataTemplate for us
        }

        // Check if we can accept the provided data
        public bool Match(object data)
        {
            // Our Keys in the dictionary are strings, so we call .ToString() to get the key to look up
            var key = data.ToString();

            return data is ShapeType                       // the provided data needs to be our enum type
                   && !string.IsNullOrEmpty(key)           // and the key must not be null or empty
                   && AvailableTemplates.ContainsKey(key); // and the key must be found in our Dictionary
        }
    } 
}
