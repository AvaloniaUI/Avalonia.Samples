namespace AdvancedToDoList.Models;

/// <summary>
/// Defines the calculated status of ToDo items based on progress and due date.
/// These statuses are automatically computed and help users quickly understand
/// the current state of their tasks without manual inspection.
/// </summary>
public enum ToDoItemStatus
{
    /// <summary>
    /// The ToDo item has not been started yet.
    /// Progress is 0% and the item is not past its due date.
    /// Typically displayed with neutral styling in the UI.
    /// </summary>
    NotStarted,
    
    /// <summary>
    /// The ToDo item is currently being worked on.
    /// Progress is between 0% and 100% and the item is not overdue.
    /// Usually displayed with active/in-progress visual indicators.
    /// </summary>
    InProgress,
    
    /// <summary>
    /// The ToDo item has been completed.
    /// Progress is 100%, regardless of due date.
    /// Often displayed with checkmarks or completion styling.
    /// </summary>
    Done,
    
    /// <summary>
    /// The ToDo item is past its due date and not yet completed.
    /// Progress is less than 100% and the due date is before the current date.
    /// Typically highlighted with warning styling to draw attention.
    /// </summary>
    Overdue
}