using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

public class MainViewModel : ViewModelBase, IDialogParticipant
{
    public ManageCategoriesViewModel CategoriesViewModel { get; } = new();
    
    public ManageToDoItemsViewModel ToDoItemsViewModel { get; } = new();
}