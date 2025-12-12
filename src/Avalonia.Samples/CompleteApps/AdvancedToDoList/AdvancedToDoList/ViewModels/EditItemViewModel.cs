using System.Threading.Tasks;
using AdvancedToDoList.ViewModels.Shared;

namespace AdvancedToDoList.ViewModels;

public partial class EditItemViewModel<T> : ViewModelBase where T : class, ISavable
{
    public EditItemViewModel(T item)
    {
        Item = item;
    }

    public T Item { get; set; }
    
    public async Task<T?> SaveAsync()
    {
        var success = await Item.SaveAsync();

        return success ? Item : null;
    }
}