using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AdvancedToDoList.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SharedControls.Controls;
using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", 
    Justification = "DynamicallyAccessedMembers is used to suppress a warning about a property that is set by reflection.")]
[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2112:RequiresUnreferencedCode",
    Justification = "DynamicallyAccessedMembers is used to suppress a warning about a property that is set by reflection.")]
public partial class EditCategoryViewModel : ViewModelBase, IDialogParticipant
{
    private readonly IDialogService _dialogService;
    private readonly ICategoryService _categoryService;

    public EditCategoryViewModel() : this(new CategoryViewModel())
    {
    }
    
    public EditCategoryViewModel(CategoryViewModel category) 
        : this(category, 
            App.Services?.GetService<ICategoryService>() ?? new CategoryService(),
            null) // DialogService needs the participant (this), so we initialize it in the constructor body if null
    {
    }

    public EditCategoryViewModel(CategoryViewModel category, ICategoryService categoryService, IDialogService? dialogService)
    {
        Item = category;
        _categoryService = categoryService;
        _dialogService = dialogService ?? new DialogService(this);
    }

    public CategoryViewModel Item { get; }
    
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
            await _dialogService.ShowOverlayDialogAsync<DialogResult>("Error", "Please correct the errors in the form.", DialogCommands.Ok);
            return;
        }
        
        var category = Item.ToCategory();
        var success = await _categoryService.SaveCategoryAsync(category);

        if (success)
        {
            _dialogService.ReturnResultFromOverlayDialog(new CategoryViewModel(category));
        }
        else
        {
            await _dialogService.ShowOverlayDialogAsync<DialogResult>("Error", "An error occurred while saving the category.", DialogCommands.Ok);
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