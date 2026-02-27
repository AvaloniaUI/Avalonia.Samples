using System.Threading.Tasks;
using AdvancedToDoList.Models;

namespace AdvancedToDoList.Services;

/// <summary>
/// Service implementation for managing category persistence operations.
/// Provides abstraction layer between ViewModels and the Category model's database operations.
/// Implements repository pattern for better testability and separation of concerns.
/// </summary>
public class CategoryService : ICategoryService
{
    /// <summary>
    /// Asynchronously saves a category to the database.
    /// Delegates to the Category model's SaveAsync method for actual persistence logic.
    /// Returns success status based on database operation result.
    /// </summary>
    /// <param name="category">The category to save (insert or update)</param>
    /// <returns>True if save was successful, false otherwise</returns>
    public async Task<bool> SaveCategoryAsync(Category category)
    {
        return await category.SaveAsync();
    }
    
    /// <summary>
    /// Asynchronously deletes a category from the database.
    /// Delegates to the Category model's DeleteAsync method for actual deletion logic.
    /// Returns success status based on database operation result.
    /// </summary>
    /// <param name="category">The category to delete</param>
    /// <returns>True if deletion was successful, false otherwise</returns>
    public async Task<bool> DeleteCategoryAsync(Category category)
    {
        return await category.DeleteAsync();
    }
}
