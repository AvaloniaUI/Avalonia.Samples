using System.Threading.Tasks;
using AdvancedToDoList.Models;

namespace AdvancedToDoList.Services;

/// <summary>
/// This is a service that handles <see cref="Category"/>-I/O operations
/// </summary>
public interface ICategoryService
{    
    /// <summary>
    /// Saves the given Category async.
    /// </summary>
    /// <param name="category">The category to save</param>
    /// <returns>True if the operation was successful, otherwise false</returns>
    Task<bool> SaveCategoryAsync(Category category);
    
    /// <summary>
    /// Deletes the given Category async.
    /// </summary>
    /// <param name="category">The category to delete</param>
    /// <returns>True if the operation was successful, otherwise false</returns>
    Task<bool> DeleteCategoryAsync(Category category);
}
