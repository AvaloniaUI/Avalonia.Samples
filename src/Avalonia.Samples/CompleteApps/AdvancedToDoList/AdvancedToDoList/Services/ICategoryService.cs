using System.Threading.Tasks;
using AdvancedToDoList.Models;

namespace AdvancedToDoList.Services;

public interface ICategoryService
{
    Task<bool> SaveCategoryAsync(Category category);
    Task<bool> DeleteCategoryAsync(Category category);
}
