namespace AdvancedToDoList.Models;

/// <summary>
/// Defines the priority of the <see cref="ToDoItem"/>
/// </summary>
public enum Priority
{
    /// <summary>
    /// Low priority
    /// </summary>
    Low = 1, 
    
    /// <summary>
    /// Medium (or normal) priority
    /// </summary>
    Medium = 2,
    
    /// <summary>
    /// High priority
    /// </summary>
    High = 3
}