using System.Threading.Tasks;
using AdvancedToDoList.Models;

namespace AdvancedToDoList.Services;

/// <summary>
/// Service interface for managing ToDoItem persistence operations.
/// Defines contract for ToDoItem CRUD operations to enable dependency injection and testing.
/// Allows swapping implementations for different platforms or testing scenarios.
/// </summary>
public interface IToDoItemService
{   
    /// <summary>
    /// Asynchronously saves a ToDo item to persistent storage.
    /// Handles both creating new items and updating existing items.
    /// </summary>
    /// <param name="item">The ToDo item to save</param>
    /// <returns>True if the operation was successful, otherwise false</returns>
    Task<bool> SaveToDoItemAsync(ToDoItem item);
    
    /// <summary>
    /// Asynchronously deletes a ToDo item from persistent storage.
    /// Permanently removes the item from the data store.
    /// </summary>
    /// <param name="item">The ToDo item to delete</param>
    /// <returns>True if the operation was successful, otherwise false</returns>
    Task<bool> DeleteToDoItemAsync(ToDoItem item);
}
