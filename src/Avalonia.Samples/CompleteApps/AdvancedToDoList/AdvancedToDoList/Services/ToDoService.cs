using System.Threading.Tasks;
using AdvancedToDoList.Models;

namespace AdvancedToDoList.Services;

public class ToDoService : IToDoService
{
    public async Task<bool> SaveToDoItemAsync(ToDoItem item)
    {
        return await item.SaveAsync();
    }

    public async Task<bool> DeleteToDoItemAsync(ToDoItem item)
    {
        return await item.DeleteAsync();
    }
}
