using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
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
        
    }
    
    public EditCategoryViewModel(Category category)
    {
        Id = category.Id;
        Name = category.Name;
        Description = category.Description;
        
        if (category.GroupColorHex != null) 
            GroupColor = Color.Parse(category.GroupColorHex);
    }

    /// <summary>
    /// Gets the Id of the Category. This property is read-only.
    /// </summary>
    [ObservableProperty]
    public partial int? Id { get; private set; }
    
    /// <summary>
    /// Gets or sets the Name of the Category. This property is required.
    /// </summary>
    [ObservableProperty, NotifyPropertyChangedFor(nameof(PreviewCategory))]
    [NotifyDataErrorInfo]
    [Required]
    public partial string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets the Description of the Category.
    /// </summary>
    [ObservableProperty, NotifyPropertyChangedFor(nameof(PreviewCategory))]
    public partial string? Description { get; set; }
    
    /// <summary>
    /// GeTs or sets the Color of the Category.
    /// </summary>
    [ObservableProperty, NotifyPropertyChangedFor(nameof(PreviewCategory))]
    public partial Color? GroupColor { get; set; }
    
    /// <summary>
    /// Gets a preview of the Category.
    /// </summary>
    public Category PreviewCategory => ToCategory();

    public Category ToCategory()
    {
        return new Category()
        {
            Id = Id,
            Name = Name,
            Description = Description,
            GroupColorHex = GroupColor?.ToString()
        };
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
            await this.ShowOverlayDialogAsync<DialogResult>("Error", "Please correct the errors in the form.", DialogCommands.Ok);
            return;
        }
        
        var category = ToCategory();
        var success = await category.SaveAsync();

        if (success)
        {
            this.ReturnResultFromOverlayDialog(category);
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