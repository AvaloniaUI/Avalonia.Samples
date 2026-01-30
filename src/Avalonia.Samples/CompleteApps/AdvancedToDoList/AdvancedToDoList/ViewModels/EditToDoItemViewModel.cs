using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedToDoList.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SharedControls.Controls;
using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

/// <summary>
/// This ViewModel provides the necessary interactions to edit a to-do-item. 
/// </summary>
public partial class EditToDoItemViewModel : ViewModelBase, IDialogParticipant
{    
    /// <summary>
    /// The <see cref="IDialogService"/> to use (needed for Unit-tests).
    /// </summary>
    private readonly IDialogService _dialogService;
    
    /// <summary>
    /// The <see cref="IToDoService"/> to use (needed for Unit-tests).
    /// </summary>
    private readonly IToDoService _toDoService;
    
    /// <summary>
    /// Creates a new instance of this ViewModel to edit the given to-do-item.
    /// </summary>
    /// <param name="toDoItem">The item to edit.</param>
    /// <param name="availableCategories">A list of available catorgires to select from.</param>
    public EditToDoItemViewModel(ToDoItemViewModel toDoItem, IList<CategoryViewModel> availableCategories)
        : this(toDoItem, availableCategories, 
            App.Services.GetService<IToDoService>() ?? new ToDoService(),
            null)
    {
    }

    /// <summary>
    /// Creates a new instance of this ViewModel to edit the given to-do-item with the provided services.
    /// </summary>
    /// <param name="toDoItem">The item to edit.</param>
    /// <param name="availableCategories">A list of available catorgires to select from.</param>
    /// <param name="toDoService">The <see cref="IDialogService"/> to use.</param>
    /// <param name="dialogService">The <see cref="IToDoService"/> to use.</param>
    public EditToDoItemViewModel(ToDoItemViewModel toDoItem, IList<CategoryViewModel> availableCategories, 
        IToDoService toDoService, IDialogService? dialogService)
    {
        Item = toDoItem;
        _toDoService = toDoService;
        _dialogService = dialogService ?? new DialogService(this);
        
        //Add the empty category to the list. This will be the first item in the list.
        availableCategories.Insert(0, CategoryViewModel.Empty);
        
        AvailableCategories = availableCategories;
    }

    /// <summary>
    /// Gets the Item that should be edited.
    /// </summary>
    public ToDoItemViewModel Item { get; }

    /// <summary>
    /// Gets a list of available categories to choose from.
    /// </summary>
    public IList<CategoryViewModel> AvailableCategories { get; }

    /// <summary>
    /// Gets a command that can be used to add a new category.
    /// </summary>
    [RelayCommand]
    private async Task AddNewCategoryAsync()
    {
        // Show an edit dialog to create a new category.
        var category = new EditCategoryViewModel(new CategoryViewModel());
        var result = await _dialogService.ShowOverlayDialogAsync<CategoryViewModel>("Add a new category", category);
        
        // if we got a new category, add it to the list and set it as the selected category.
        if (result != null)
        {
            AvailableCategories.Add(result);
            Item.Category = result;
        }
    }

    /// <summary>
    /// Gets a command that resets the category to the default (<see cref="CategoryViewModel.Empty"/>) category.
    /// </summary>
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
        // Check if the item is valid. If the item has errors, notify the user and exit this method.
        Item.Validate();
        if (Item.HasErrors)
        {
            await _dialogService.ShowOverlayDialogAsync<DialogResult>("Error", "Please correct the errors in the form.",
                DialogCommands.Ok);
            return;
        }
        
        // try to save the item.
        var toDoItem = Item.ToToDoItem();
        var success = await _toDoService.SaveToDoItemAsync(toDoItem);
        
        // If saving was successful, return the changed item, otherwise show an error dialog.
        if (success)
        {
            // NOTE: We have to create a new CategoryViewModel here, since the save method may have changed some properties
            // (for example: the ID)
            _dialogService.ReturnResultFromOverlayDialog(new ToDoItemViewModel(toDoItem));
        }
        else
        {
            // Show an error message if something went wrong.
            await _dialogService.ShowOverlayDialogAsync<bool>("Error", "An error occurred while saving the to-do item.",
                DialogCommands.Ok);
        }
    }

    /// <summary>
    /// Gets a command that cancels the edit. 
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">This exception should never occur in theory.</exception>
    [RelayCommand]
    private async Task CancelAsync()
    {
        var userResponse = await _dialogService.ShowOverlayDialogAsync<DialogResult>(
            "Save changes?", 
            "Do you want to save the changes before closing this dialog?", 
            DialogCommands.YesNoCancel);

        switch (userResponse)
        {
            case DialogResult.Yes:
                // This will also automatically close the dialog.
                await this.SaveAsync();
                break;
            case DialogResult.No:
                // Return null to indicate that no changes were saved.
                _dialogService.ReturnResultFromOverlayDialog(null);
                break;
            case DialogResult.Cancel:
                // Do nothing if the dialog was canceled
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}