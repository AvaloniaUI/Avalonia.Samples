using System.Threading.Tasks;
using AdvancedToDoList.Models;
using AdvancedToDoList.Helper;

namespace AdvancedToDoList.Services;

public class CategoryService : ICategoryService
{
    public async Task<bool> SaveCategoryAsync(Category category)
    {
        return await category.SaveAsync();
    }

    public async Task<bool> DeleteCategoryAsync(Category category)
    {
        return await category.DeleteAsync();
    }
}
