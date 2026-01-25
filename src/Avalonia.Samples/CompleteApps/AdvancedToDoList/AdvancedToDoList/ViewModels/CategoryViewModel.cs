using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using AdvancedToDoList.Helper;
using AdvancedToDoList.Models;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AdvancedToDoList.ViewModels;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public partial class CategoryViewModel : ViewModelBase, IEquatable<CategoryViewModel>, ICloneable
{
    public static CategoryViewModel Empty { get; } = new CategoryViewModel()
    {
        Name = "Uncategorized",
        Color = Color.FromArgb(255, 150, 150, 150),
    };
    
    public CategoryViewModel()
    {
        Color = ColorHelper.GetRandomColor();
        Name = "New Category";
    }
    
    public CategoryViewModel(Category category)
    {
        Id = category.Id;
        Name = category.Name;
        Description = category.Description;
        Color = Color.TryParse(category.Color, out var color) 
            ? color 
            : ColorHelper.GetRandomColor();
    }
    
    /// <summary>
    /// Gets the ID of the Category
    /// </summary>
    public long? Id { get; }
    
    /// <summary>
    /// Gets the Name of the Category
    /// </summary>
    [ObservableProperty]
    [Required]
    public partial string? Name { get; set; }
    
    /// <summary>
    /// Gets the Description of the Category
    /// </summary>
    [ObservableProperty]
    public partial string? Description { get; set; }
    
    /// <summary>
    /// Gets the Color of the Category
    /// </summary>
    /// <remarks>
    /// We use the Avalonia.Media namespace here directly, which in theory breaks the MVVM-pattern.
    /// The alternative would be to use a Converter that parses the Color name each time we access this item. 
    /// </remarks>
    [ObservableProperty]
    public partial Color Color { get; set; }

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

    public bool Equals(CategoryViewModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((CategoryViewModel)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public object Clone()
    {
        return MemberwiseClone();
    }

    public static bool operator ==(CategoryViewModel? left, CategoryViewModel? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(CategoryViewModel? left, CategoryViewModel? right)
    {
        return !Equals(left, right);
    }
}