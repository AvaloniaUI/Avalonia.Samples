using AdvancedToDoList.Helper;
using AdvancedToDoList.Models;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;

namespace AdvancedToDoList.DataTemplates;

/// <summary>
/// This class is a helper to resolve the symbol-representation of our <see cref="Priority"/>-enum
/// </summary>
public class ToDoItemPriorityIconTemplateSelector : IDataTemplate
{
    /// <summary>
    /// Gets the default instance of this class
    /// </summary>
    public static ToDoItemPriorityIconTemplateSelector Instance { get; } = new ToDoItemPriorityIconTemplateSelector();
    
    /// <inheritdoc />
    public Control? Build(object? param)
    {
        var priority = param as Priority?;
        var fontSize = ResourcesHelper.GetAppResource<double>("ControlContentThemeFontSize");
        
        return priority switch
        {
            Priority.Low =>
                new PathIcon()
                {
                    Data = ResourcesHelper.GetAppResource<Geometry>("PhosphorIcons.ArrowCircleDownRight"),
                    Opacity = 0.5,
                    Width = fontSize, 
                    Height = fontSize
                },

            Priority.Medium => 
                new PathIcon()
                {
                    Data = ResourcesHelper.GetAppResource<Geometry>("PhosphorIcons.ArrowCircleRight"),
                    Width = fontSize, 
                    Height = fontSize
                },
            
            Priority.High =>
                new PathIcon()
                {
                    Data = ResourcesHelper.GetAppResource<Geometry>("PhosphorIcons.ArrowCircleUpRight"),
                    Foreground = Brushes.Red,
                    Width = fontSize, 
                    Height = fontSize
                },
            
            _ => null
        };
    }

    /// <inheritdoc />
    public bool Match(object? data)
    {
        return data is Priority;
    }
}