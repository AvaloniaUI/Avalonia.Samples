using AdvancedToDoList.Helper;
using AdvancedToDoList.Models;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;

namespace AdvancedToDoList.DataTemplates;

/// <summary>
/// This class is a helper to resolve the symbol for the <see cref="ToDoItemStatus"/>.
/// </summary>
public class ToDoItemStatusTemplateSelector : IDataTemplate
{
    /// <summary>
    /// Gets the default instance of this class
    /// </summary>
    public static ToDoItemStatusTemplateSelector Instance { get; } = new ToDoItemStatusTemplateSelector();

    /// <inheritdoc />
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
                    // TIP: If you want your App to be localized, you could also provide this text via App.Resources
                    // or any other localization provider.
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

    /// <inheritdoc />
    public bool Match(object? data)
    {
        return data is ToDoItemStatus;
    }
}