using AdvancedToDoList.Helper;
using AdvancedToDoList.Models;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;

namespace AdvancedToDoList.DataTemplates;

public class ToDoItemPriorityIconTemplateSelector : IDataTemplate
{
    public static ToDoItemPriorityIconTemplateSelector Instance { get; } = new ToDoItemPriorityIconTemplateSelector();
    
    public Control? Build(object? param)
    {
        var priority = param as Priority?;
        var fontSize = ResourcesHelper.GetAppResource<double>("ControlContentThemeFontSize");
        
        return priority switch
        {
            Priority.Low =>
                new PathIcon()
                {
                    Data = ResourcesHelper.GetAppResource<Geometry>("PhosphorIcons.ArrowCircleDownRightLight"),
                    Opacity = 0.5,
                    Width = fontSize, 
                    Height = fontSize
                },

            Priority.Medium => 
                new PathIcon()
                {
                    Data = ResourcesHelper.GetAppResource<Geometry>("PhosphorIcons.ArrowCircleRightLight"),
                    Width = fontSize, 
                    Height = fontSize
                },
            
            Priority.High =>
                new PathIcon()
                {
                    Data = ResourcesHelper.GetAppResource<Geometry>("PhosphorIcons.ArrowCircleUpRightLight"),
                    Foreground = new SolidColorBrush(Colors.Red),
                    Width = fontSize, 
                    Height = fontSize
                },
            
            _ => null
        };
    }    
    
    public bool Match(object? data)
    {
        return data is Priority;
    }
}