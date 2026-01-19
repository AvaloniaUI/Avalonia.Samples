using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedToDoList.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SharedControls.Controls;
using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

public partial class EditToDoItemViewModel : ViewModelBase, IDialogParticipant
{
    private readonly IDialogService _dialogService;
    private readonly IToDoService _toDoService;

    public EditToDoItemViewModel(ToDoItemViewModel toDoItem, IList<CategoryViewModel> availableCategories)
        : this(toDoItem, availableCategories, 
            App.Services?.GetService<IToDoService>() ?? new ToDoService(),
            null)
    {
    }

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

    public ToDoItemViewModel Item { get; }

    // ToDo: Make this async? 
    public IList<CategoryViewModel> AvailableCategories { get; }

    [RelayCommand]
    private async Task AddNewCategoryAsync()
    {
        var category = new EditCategoryViewModel(new CategoryViewModel());
        var result = await _dialogService.ShowOverlayDialogAsync<CategoryViewModel>("Add a new category", category);
        
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
        // Check if the item is valid
        Item.Validate();
        if (Item.HasErrors)
        {
            await _dialogService.ShowOverlayDialogAsync<DialogResult>("Error", "Please correct the errors in the form.",
                DialogCommands.Ok);
            return;
        }

        var toDoItem = Item.ToToDoItem();
        var success = await _toDoService.SaveToDoItemAsync(toDoItem);

        if (success)
        {
            _dialogService.ReturnResultFromOverlayDialog(new ToDoItemViewModel(toDoItem));
        }
        else
        {
            await _dialogService.ShowOverlayDialogAsync<bool>("Error", "An error occurred while saving the to-do item.",
                DialogCommands.Ok);
        }
    }

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