using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AdvancedToDoList.Models;
using AdvancedToDoList.ViewModels.Shared;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AdvancedToDoList.ViewModels;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public partial class CategoryViewModel : ViewModelBase, ISavable
{
    public CategoryViewModel()
    {
        
    }
    
    public CategoryViewModel(Category category)
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
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    public partial string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets the Description of the Category.
    /// </summary>
    [ObservableProperty]
    public partial string? Description { get; set; }
    
    /// <summary>
    /// GeTs or sets the Color of the Category.
    /// </summary>
    [ObservableProperty]
    public partial Color? GroupColor { get; set; }

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
    
    public async Task<bool> SaveAsync()
    {
        var category = ToCategory();
        await category.SaveAsync();
        
        Id = category.Id;
        
        return true;
    }
}