using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

public partial class MainViewModel : ViewModelBase, IDialogParticipant
{
    public CategoriesViewModel CategoriesViewModel { get; } = new();
    
    public ToDoItemsViewModel ToDoItemsViewModel { get; } = new();
}