using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using AdvancedToDoList.Helper;
using AdvancedToDoList.Models;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AdvancedToDoList.ViewModels;

/// <summary>
/// This ViewModel represents a single <see cref="Category"/>.
/// </summary>
/// <remarks>
/// We annotate this class with <see cref="DynamicallyAccessedMembersAttribute"/>,
/// so that the trimmer knows which items to preserve.
/// </remarks>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
[UnconditionalSuppressMessage("Trimming", "IL2112", Justification = "We have all needed members added via DynamicallyAccessedMembers-Attribute")]
[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "We have all needed members added via DynamicallyAccessedMembers-Attribute")]
public partial class CategoryViewModel : ViewModelBase, IEquatable<CategoryViewModel>, ICloneable
{
    /// <summary>
    /// This is the representation if no Category is set.
    /// </summary>
    public static CategoryViewModel Empty { get; } = new CategoryViewModel()
    {
        Name = "Uncategorized",
        Color = Color.FromArgb(255, 150, 150, 150),
    };

    /// <summary>
    /// Creates a new instance with a random color.
    /// </summary>
    public CategoryViewModel()
    {
        Color = ColorHelper.GetRandomColor();
        Name = "New Category";
    }
    
    /// <summary>
    /// Creates a new instance for the provided <see cref="Category"/>
    /// </summary>
    /// <param name="category">The category to represent.</param>
    public CategoryViewModel(Category category) : this()
    {
        Id = category.Id;
        Name = category.Name;
        Description = category.Description;
        Color = Color.TryParse(category.Color, out var color) 
            ? color 
            : ColorHelper.GetRandomColor();
    }
    
    /// <summary>
    /// Gets the ID of the Category.
    /// </summary>
    public long? Id { get; }
    
    /// <summary>
    /// Gets the Name of the Category. 
    /// </summary>
    /// <remarks>
    /// This property is required.
    /// Remember to add "NotifyDataErrorInfo" to ensure the property will be validated.
    /// </remarks>
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    public partial string? Name { get; set; }
    
    /// <summary>
    /// Gets the Description of the Category.
    /// </summary>
    [ObservableProperty]
    public partial string? Description { get; set; }
    
    /// <summary>
    /// Gets the Color of the Category
    /// </summary>
    /// <remarks>
    /// We use the Avalonia.Media namespace here directly, which in theory breaks the MVVM pattern.
    /// The alternative would be to use a Converter that parses the Color name each time we access this item. 
    /// </remarks>
    [ObservableProperty]
    public partial Color Color { get; set; }

    /// <summary>
    /// Converts this instance to the underlying model.
    /// </summary>
    /// <returns>the <see cref="Category"/>-representation</returns>
    public Category ToCategory()
    {
        return new Category()
        {
            Id = Id,
            Name = Name,
            Description = Description,
            Color = Color.ToString()
        };
    }
    
    // -- IEquatable implementation
    /// <inheritdoc />
    public bool Equals(CategoryViewModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }
    
    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((CategoryViewModel)obj);
    }
    
    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    
    public static bool operator ==(CategoryViewModel? left, CategoryViewModel? right)
    {
        return Equals(left, right);
    }
    
    public static bool operator !=(CategoryViewModel? left, CategoryViewModel? right)
    {
        return !Equals(left, right);
    }
    
    /// <inheritdoc />
    public object Clone()
    {
        return MemberwiseClone();
    }

}