using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedToDoList.Helper;
using AdvancedToDoList.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharedControls.Controls;
using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

public partial class EditToDoItemViewModel : ViewModelBase, IDialogParticipant
{
    private readonly bool _isInitialized;

    public EditToDoItemViewModel(ToDoItemViewModel toDoItem, IList<CategoryViewModel> availableCategories)
    {
        Item = toDoItem;
        
        //Add the empty category to the list. This will be the first item in the list.
        availableCategories.Insert(0, CategoryViewModel.Empty);
        
        AvailableCategories = availableCategories;
    }

    public ToDoItemViewModel Item { get; }

    // ToDo: Make this async? 
    public IList<CategoryViewModel> AvailableCategories { get; }

    [RelayCommand]
    private async Task AddNewCategoryAsync()
    {
        var category = new EditCategoryViewModel(new CategoryViewModel());
        var result = await this.ShowOverlayDialogAsync<CategoryViewModel>("Add a new category", category);
        
        if (result != null)
        {
            AvailableCategories.Add(result);
            Item.Category = result;
        }
    }

    [RelayCommand]
    private void SetCategoryToEmpty()
    {
        Item.Category = CategoryViewModel.Empty;
    }
    
    /// <summary>
    /// Gets a command that saves the changes to the item and returns the item to the caller.
    /// </summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        // Check if the category is valid
        if (HasErrors)
        {
            await this.ShowOverlayDialogAsync<DialogResult>("Error", "Please correct the errors in the form.",
                DialogCommands.Ok);
            return;
        }

        var toDoItem = Item.ToToDoItem();
        var success = await toDoItem.SaveAsync();

        if (success)
        {
            this.ReturnResultFromOverlayDialog(new ToDoItemViewModel(toDoItem));
        }
        else
        {
            await this.ShowOverlayDialogAsync<bool>("Error", "An error occurred while saving the to-do item.",
                DialogCommands.Ok);
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        // TODO ask user for confirmation
        this.ReturnResultFromOverlayDialog(null);
    }
}