using System.Threading.Tasks;
using AdvancedToDoList.Models;

namespace AdvancedToDoList.Services;

/// <summary>
/// This is a service that handles <see cref="ToDoItem"/>-I/O operations
/// </summary>
public interface IToDoService
{   
    /// <summary>
    /// Saves the given ToDoItem async.
    /// </summary>
    /// <param name="item">The ToDoItem to save</param>
    /// <returns>True if the operation was successful, otherwise false</returns>
    Task<bool> SaveToDoItemAsync(ToDoItem item);
    
    /// <summary>
    /// Deletes the given ToDoItem async.
    /// </summary>
    /// <param name="item">The ToDoItem to delete</param>
    /// <returns>True if the operation was successful, otherwise false</returns>
    Task<bool> DeleteToDoItemAsync(ToDoItem item);
}
