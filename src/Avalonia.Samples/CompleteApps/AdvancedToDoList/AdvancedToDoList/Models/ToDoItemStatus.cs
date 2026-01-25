using System;

namespace AdvancedToDoList.Models;

/// <summary>
/// Defines the calculated ToDoItem-status
/// </summary>
public enum ToDoItemStatus
{
    /// <summary>
    /// The <see cref="ToDoItem.Progress"/> is <c>0</c> and the item is not <see cref="Overdue"/>
    /// </summary>
    NotStarted,
    
    /// <summary>
    /// The <see cref="ToDoItem.Progress"/> is between <c>0</c> and <c>100</c> and the item is not <see cref="Overdue"/> 
    /// </summary>
    InProgress,
    
    /// <summary>
    /// The <see cref="ToDoItem.Progress"/> is <c>100</c>
    /// </summary>
    Done,
    
    /// <summary>
    /// The <see cref="ToDoItem.Progress"/> is smaller than <c>100</c> and the
    /// <see cref="ToDoItem.DueDate"/> is smaller than <see cref="DateTime.Now"/>
    /// </summary>
    Overdue
}