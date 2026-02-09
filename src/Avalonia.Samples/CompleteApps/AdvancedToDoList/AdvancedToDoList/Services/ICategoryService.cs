using System.Threading.Tasks;
using AdvancedToDoList.Models;

namespace AdvancedToDoList.Services;

/// <summary>
/// Service interface for managing category persistence operations.
/// Defines contract for category CRUD operations to enable dependency injection and testing.
/// Allows swapping implementations for different platforms or testing scenarios.
/// </summary>
public interface ICategoryService
{    
    /// <summary>
    /// Asynchronously saves a category to persistent storage.
    /// Handles both creating new categories and updating existing categories.
    /// </summary>
    /// <param name="category">The category to save</param>
    /// <returns>True if the operation was successful, otherwise false</returns>
    Task<bool> SaveCategoryAsync(Category category);
    
    /// <summary>
    /// Asynchronously deletes a category from persistent storage.
    /// Permanently removes the category from the data store.
    /// Note: Deleting a category may affect ToDo items that reference it.
    /// </summary>
    /// <param name="category">The category to delete</param>
    /// <returns>True if the operation was successful, otherwise false</returns>
    Task<bool> DeleteCategoryAsync(Category category);
}
