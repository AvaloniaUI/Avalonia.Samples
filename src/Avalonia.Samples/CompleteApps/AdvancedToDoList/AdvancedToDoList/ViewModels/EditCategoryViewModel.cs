using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AdvancedToDoList.Helper;
using AdvancedToDoList.Models;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public EditCategoryViewModel()
    {
        Item = new CategoryViewModel();
    }
    
    public EditCategoryViewModel(CategoryViewModel category)
    {
        Item = category;
    }

    public CategoryViewModel Item { get; }
    
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
        
        var category = Item.ToCategory();
        var success = await category.SaveAsync();

        if (success)
        {
            this.ReturnResultFromOverlayDialog(new CategoryViewModel(category));
        }
        else
        {
            await this.ShowOverlayDialogAsync<bool>("Error", "An error occurred while saving the category.", DialogCommands.Ok);
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        // TODO ask user for confirmation
        this.ReturnResultFromOverlayDialog(null);
    }
}