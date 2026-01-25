using System.Threading.Tasks;
using AdvancedToDoList.Models;

namespace AdvancedToDoList.Services;

/// <summary>
/// This is a service that handles <see cref="ToDoItem"/>-I/O operations
/// </summary>
public class ToDoService : IToDoService
{    
    /// <inheritdoc />
    public async Task<bool> SaveToDoItemAsync(ToDoItem item)
    {
        return await item.SaveAsync();
    }
    
    /// <inheritdoc />
    public async Task<bool> DeleteToDoItemAsync(ToDoItem item)
    {
        return await item.DeleteAsync();
    }
}
