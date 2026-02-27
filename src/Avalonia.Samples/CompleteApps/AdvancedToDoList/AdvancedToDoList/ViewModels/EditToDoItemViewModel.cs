using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedToDoList.Messages;
using AdvancedToDoList.Models;
using AdvancedToDoList.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SharedControls.Controls;
using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

/// <summary>
/// ViewModel for editing ToDoItems in a dialog context.
/// Provides UI interactions for modifying ToDoItem properties, managing categories,
/// and validating input before saving changes to the database.
/// </summary>
public partial class EditToDoItemViewModel : ViewModelBase, IDialogParticipant
{    
    /// <summary>
    /// Dialog service for showing dialogs and managing user interactions.
    /// Used for confirmation dialogs, error messages, and category creation.
    /// </summary>
    private readonly IDialogService _dialogService;
    
    /// <summary>
    /// Service for saving and managing ToDoItem data.
    /// Handles persistence of ToDoItems to the database.
    /// </summary>
    private readonly IToDoItemService _toDoService;
    
    /// <summary>
    /// Creates a new instance of this ViewModel to edit the given ToDoItem.
    /// Resolves services from the dependency injection container or creates default instances.
    /// </summary>
    /// <param name="toDoItem">The ToDoItem to edit.</param>
    /// <param name="availableCategories">A list of available categories to select from.</param>
    public EditToDoItemViewModel(ToDoItemViewModel toDoItem, IList<CategoryViewModel> availableCategories)
        : this(toDoItem, availableCategories, 
            App.Services.GetService<IToDoItemService>() ?? new ToDoItemService(),
            null)
    {
    }

    /// <summary>
    /// Creates a new instance of this ViewModel to edit the given to-do-item with the provided services.
    /// This constructor is primarily used for unit testing with mocked services.
    /// </summary>
    /// <param name="toDoItem">The ToDoItem to edit.</param>
    /// <param name="availableCategories">A list of available categories to select from.</param>
    /// <param name="toDoService">The service for managing ToDoItem persistence.</param>
    /// <param name="dialogService">The service for managing dialog interactions.</param>
    public EditToDoItemViewModel(ToDoItemViewModel toDoItem, IList<CategoryViewModel> availableCategories, 
        IToDoItemService toDoService, IDialogService? dialogService)
    {
        Item = toDoItem;
        _toDoService = toDoService;
        _dialogService = dialogService ?? new DialogService(this);
        
        //Add the empty category to the list as the first option (represents "uncategorized")
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
    /// Command that opens a dialog to create a new category.
    /// The newly created category is added to the available categories list
    /// and automatically selected for the current ToDoItem.
    /// </summary>
    [RelayCommand]
    private async Task AddNewCategoryAsync()
    {
        // Show an edit dialog to create a new category
        var category = new EditCategoryViewModel(new CategoryViewModel());
        var result = await _dialogService.ShowOverlayDialogAsync<CategoryViewModel>("Add a new category", category);
        
        // If the user successfully created a new category, add it to the list and select it
        if (result != null)
        {
            UpdateDataMessage<Category>.CreateAndSend(UpdateAction.Added, result.ToCategory());
            AvailableCategories.Add(result);
            Item.Category = result;
        }
    }

    /// <summary>
    /// Command that resets the ToDoItem's category to the default "uncategorized" option.
    /// Uses the static Empty category to represent items without a specific category.
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
            await _dialogService.ShowOverlayDialogAsync<DialogResult>(
                "Error", 
                "Please correct the errors in the form.",
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
            // (for example, the ID)
            _dialogService.ReturnResultFromOverlayDialog(new ToDoItemViewModel(toDoItem));
        }
        else
        {
            // Show an error message if something went wrong.
            await _dialogService.ShowOverlayDialogAsync<bool>(
                "Error", 
                "An error occurred while saving the to-do item.",
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