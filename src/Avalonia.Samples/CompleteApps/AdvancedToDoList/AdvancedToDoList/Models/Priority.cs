namespace AdvancedToDoList.Models;

/// <summary>
/// Defines the priority levels for ToDo items.
/// Higher numeric values indicate higher priority for sorting purposes.
/// These priorities help users organize and focus on the most important tasks.
/// </summary>
public enum Priority
{
    /// <summary>
    /// Low priority for less urgent or optional tasks.
    /// Typically displayed with minimal visual emphasis in the UI.
    /// </summary>
    Low = 1, 
    
    /// <summary>
    /// Medium (or normal) priority for standard tasks.
    /// This is the default priority level for newly created ToDo items.
    /// Represents the baseline importance for regular tasks.
    /// </summary>
    Medium = 2,
    
    /// <summary>
    /// High priority for urgent or important tasks.
    /// Typically displayed with prominent visual indicators in the UI.
    /// Used to draw attention to tasks that need immediate focus.
    /// </summary>
    High = 3
}