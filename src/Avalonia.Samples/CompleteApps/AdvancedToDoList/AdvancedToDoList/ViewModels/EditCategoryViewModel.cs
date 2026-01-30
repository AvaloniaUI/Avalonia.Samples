using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AdvancedToDoList.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SharedControls.Controls;
using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

/// <summary>
/// This ViewModel provides the necessary interactions to edit a category. 
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", 
    Justification = "DynamicallyAccessedMembers is used to suppress a warning about a property that is set by reflection.")]
[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2112:RequiresUnreferencedCode",
    Justification = "DynamicallyAccessedMembers is used to suppress a warning about a property that is set by reflection.")]
public partial class EditCategoryViewModel : ViewModelBase, IDialogParticipant
{
    /// <summary>
    /// The <see cref="IDialogService"/> to use (needed for Unit-tests).
    /// </summary>
    private readonly IDialogService _dialogService;
    
    /// <summary>
    /// The <see cref="ICategoryService"/> to use (needed for Unit-tests).
    /// </summary>
    private readonly ICategoryService _categoryService;

    /// <summary>
    /// Parameterless constructor for the Designer
    /// </summary>
    public EditCategoryViewModel() : this(new CategoryViewModel())
    {
    }
    
    /// <summary>
    /// Creates a new instance of this ViewModel to edit the given category.
    /// </summary>
    /// <param name="category">The category to edit.</param>
    public EditCategoryViewModel(CategoryViewModel category) 
        : this(category, 
            App.Services?.GetService<ICategoryService>() ?? new CategoryService(),
            null) // DialogService needs the participant (this), so we initialize it in the constructor body if null
    {
    }

    /// <summary>
    /// Creates a new instance of this ViewModel to edit the given category and services.
    /// </summary>
    /// <param name="category">The category to edit.</param>
    /// <param name="categoryService">The <see cref="ICategoryService"/> to use.</param>
    /// <param name="dialogService">The <see cref="IDialogService"/> to use.</param>
    public EditCategoryViewModel(CategoryViewModel category, ICategoryService categoryService, IDialogService? dialogService)
    {
        Item = category;
        _categoryService = categoryService;
        _dialogService = dialogService ?? new DialogService(this);
    }

    /// <summary>
    /// Gets the Item that should be edited.
    /// </summary>
    public CategoryViewModel Item { get; }
    
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
            await _dialogService.ShowOverlayDialogAsync<DialogResult>("Error", "Please correct the errors in the form.", DialogCommands.Ok);
            return;
        }
        
        // try to save the item.
        var category = Item.ToCategory();
        var success = await _categoryService.SaveCategoryAsync(category);

        // If saving was successful, return the changed item, otherwise show an error dialog.
        if (success)
        {
            // NOTE: We have to create a new CategoryViewModel here, since the save method may have changed some properties
            // (for example: the ID)
            _dialogService.ReturnResultFromOverlayDialog(new CategoryViewModel(category));
        }
        else
        {
            // Show an error message if something went wrong.
            await _dialogService.ShowOverlayDialogAsync<DialogResult>("Error", "An error occurred while saving the category.", DialogCommands.Ok);
        }
    }

    /// <summary>
    /// Gets a command that cancels the edit. 
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">This exception should never occur in theory.</exception>
    [RelayCommand]
    private async Task CancelAsync()
    {
        // Ask the user if they really want to dismiss their changes. In production, we may want to track changes and only 
        // show this dialog if there really was a change. 
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