using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

public partial class MainViewModel : ViewModelBase, IDialogParticipant
{
    public ManageCategoriesViewModel CategoriesViewModel { get; } = new();
    
    public ManageToDoItemsViewModel ToDoItemsViewModel { get; } = new();
}