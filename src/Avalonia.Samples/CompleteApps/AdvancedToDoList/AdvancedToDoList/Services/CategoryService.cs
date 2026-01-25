using System.Threading.Tasks;
using AdvancedToDoList.Models;
using AdvancedToDoList.Helper;

namespace AdvancedToDoList.Services;

/// <summary>
/// This is a service that handles <see cref="Category"/>-I/O operations
/// </summary>
public class CategoryService : ICategoryService
{
    /// <inheritdoc />
    public async Task<bool> SaveCategoryAsync(Category category)
    {
        return await category.SaveAsync();
    }
    
    /// <inheritdoc />
    public async Task<bool> DeleteCategoryAsync(Category category)
    {
        return await category.DeleteAsync();
    }
}
