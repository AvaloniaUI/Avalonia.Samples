using AdvancedToDoList.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;

namespace AdvancedToDoList.DataTemplates;

public class ToDoItemStatusTemplateSelector : IDataTemplate
{
    public static ToDoItemStatusTemplateSelector Instance { get; } = new ToDoItemStatusTemplateSelector();
    
    public Control? Build(object? param)
    {
        var status = param as ToDoItemStatus?;
        return status switch
        {
            ToDoItemStatus.NotStarted =>
                new PathIcon()
                {
                    Data = GetAppResource<Geometry>("PhosphorIcons.CircleDashedLight"),
                    Opacity = 0.7,
                    [ToolTip.TipProperty] = "Pending"
                },

            ToDoItemStatus.InProgress => 
                new PathIcon()
                {
                    Data = GetAppResource<Geometry>("PhosphorIcons.ClockCountdownLight"),
                    Opacity = 0.7,
                    [ToolTip.TipProperty] = "In Progress"
                },
            
            ToDoItemStatus.Done =>
                new PathIcon()
                {
                    Data = GetAppResource<Geometry>("PhosphorIcons.CheckCircleLight"),
                    Foreground = new SolidColorBrush(Colors.LimeGreen),
                    [ToolTip.TipProperty] = "Done"
                },
            
            ToDoItemStatus.Overdue =>
                new PathIcon()
                {
                    Data = GetAppResource<Geometry>("PhosphorIcons.WarningCircleLight"),
                    Foreground = GetAppResource<IBrush>("SystemControlErrorTextForegroundBrush"),
                    [ToolTip.TipProperty] = "Delayed"
                },
            
            _ => null
        };
    }

    private static T? GetAppResource<T>(string resourceKey)
    {
        var found = Application.Current!.TryFindResource(resourceKey, out var resource);
        if (found && resource is T value)
        {
            return value;
        }
        return default;
    }
    
    public bool Match(object? data)
    {
        return data is ToDoItemStatus;
    }
}