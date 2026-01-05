using AdvancedToDoList.Helper;
using AdvancedToDoList.Models;
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
                    Data = ResourcesHelper.GetAppResource<Geometry>("PhosphorIcons.CircleDashedLight"),
                    Opacity = 0.7,
                    [ToolTip.TipProperty] = "Pending"
                },

            ToDoItemStatus.InProgress => 
                new PathIcon()
                {
                    Data = ResourcesHelper.GetAppResource<Geometry>("PhosphorIcons.ClockCountdownLight"),
                    Opacity = 0.7,
                    [ToolTip.TipProperty] = "In Progress"
                },
            
            ToDoItemStatus.Done =>
                new PathIcon()
                {
                    Data = ResourcesHelper.GetAppResource<Geometry>("PhosphorIcons.CheckCircleLight"),
                    Foreground = new SolidColorBrush(Colors.LimeGreen),
                    [ToolTip.TipProperty] = "Done"
                },
            
            ToDoItemStatus.Overdue =>
                new PathIcon()
                {
                    Data = ResourcesHelper.GetAppResource<Geometry>("PhosphorIcons.WarningCircleLight"),
                    Foreground = ResourcesHelper.GetAppResource<IBrush>("SystemControlErrorTextForegroundBrush"),
                    [ToolTip.TipProperty] = "Delayed"
                },
            
            _ => null
        };
    }
    
    public bool Match(object? data)
    {
        return data is ToDoItemStatus;
    }
}