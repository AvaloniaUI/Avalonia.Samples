using System.Threading.Tasks;

namespace AdvancedToDoList.ViewModels.Shared;

public interface ISavable
{
    Task<bool> SaveAsync();
}