using System;
using System.ComponentModel;
using System.Threading.Tasks;
using AdvancedToDoList.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharedControls.Controls;
using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

public partial class EditToDoItemViewModel: ViewModelBase, IDialogParticipant
{
    public EditToDoItemViewModel(ToDoItem toDoItem)
    {
        Id = toDoItem.Id;
        Title = toDoItem.Title;
        Description = toDoItem.Description;
        Category = toDoItem.Category;
        Progress = toDoItem.Progress;
        Priority = (int)toDoItem.Priority;
        DueDate = toDoItem.DueDate;
        CreatedDate = toDoItem.CreatedDate;
        CompletedDate = toDoItem.CompletedDate;
    }
    
    [ObservableProperty] 
    public partial int? Id {get; set;}
    
    [ObservableProperty] 
    public partial string? Title {get; set;}
    
    [ObservableProperty] 
    public partial string? Description {get; set;}
    
    [ObservableProperty]
    public partial Category? Category {get; set;}
    
    [ObservableProperty]
    public partial int Progress { get; set; }
    
    [ObservableProperty]
    public partial int Priority { get; set; }
    
    [ObservableProperty]
    public partial DateTime DueDate { get; set; }
    
    [ObservableProperty]
    public partial DateTime CreatedDate { get; private set; }
    
    [ObservableProperty]
    public partial DateTime? CompletedDate { get; private set; }

    private ToDoItem ToToDoItem() => new ToDoItem()
    {
        Id = Id,
        Title = Title,
        Description = Description,
        CategoryId = Category?.Id,
        Category = Category,
        Progress = Progress,
        Priority = (Priority)Priority,
        DueDate = DueDate,
        CreatedDate = CreatedDate,
        CompletedDate = CompletedDate,
    };
    
    /// <summary>
    /// Gets a command that saves the changes to the item and returns the item to the caller.
    /// </summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        // Check if the category is valid
        if (HasErrors)
        {
            await this.ShowOverlayDialogAsync<DialogResult>("Error", "Please correct the errors in the form.", DialogCommands.Ok);
            return;
        }
        
        var toDoItem = ToToDoItem();
        var success = await toDoItem.SaveAsync();

        if (success)
        {
            this.ReturnResultFromOverlayDialog(toDoItem);
        }
        else
        {
            await this.ShowOverlayDialogAsync<bool>("Error", "An error occurred while saving the to-do item.", DialogCommands.Ok);
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        // TODO ask user for confirmation
        this.ReturnResultFromOverlayDialog(null);
    }
}