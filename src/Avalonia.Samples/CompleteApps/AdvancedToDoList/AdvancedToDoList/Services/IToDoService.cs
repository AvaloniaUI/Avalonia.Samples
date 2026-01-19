using System.Threading.Tasks;
using AdvancedToDoList.Models;

namespace AdvancedToDoList.Services;

public interface IToDoService
{
    Task<bool> SaveToDoItemAsync(ToDoItem item);
    Task<bool> DeleteToDoItemAsync(ToDoItem item);
}
