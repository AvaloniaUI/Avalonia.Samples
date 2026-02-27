using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AdvancedToDoList.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdvancedToDoList.ViewModels;

/// <summary>
/// ViewModel representing a single ToDoItem with comprehensive property management and validation.
/// Provides observable properties for UI binding, calculated status properties, and
/// persistence operations for ToDoItems in the application.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
[UnconditionalSuppressMessage("Trimming", "IL2112", Justification = "We have all needed members added via DynamicallyAccessedMembers-Attribute")]
[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "We have all needed members added via DynamicallyAccessedMembers-Attribute")]
public partial class ToDoItemViewModel : ViewModelBase, ICloneable
{
    /// <summary>
    /// Initializes a new ToDoItemViewModel from an existing ToDoItem model.
    /// Creates a ViewModel wrapper around the database model for UI binding.
    /// Handles category conversion and property initialization.
    /// </summary>
    /// <param name="toDoItem">The ToDoItem model to wrap in this ViewModel</param>
    public ToDoItemViewModel(ToDoItem toDoItem)
    {
        Id = toDoItem.Id;
        Category = toDoItem.Category != null 
            ? new CategoryViewModel(toDoItem.Category) 
            : CategoryViewModel.Empty;
        Title = toDoItem.Title;
        Priority = (Priority)toDoItem.Priority;
        Description = toDoItem.Description;
        DueDate = toDoItem.DueDate;
        Progress = toDoItem.Progress;
        CreatedDate = toDoItem.CreatedDate;
        CompletedDate = toDoItem.CompletedDate;
    }
    
    /// <summary>
    /// Gets the Id of the ToDoItem. This property is read-only.
    /// </summary>
    [ObservableProperty]
    public partial long? Id { get; private set; }

    /// <summary>
    /// Gets or sets the Category of the ToDoItem.
    /// </summary>
    [ObservableProperty]
    public partial CategoryViewModel Category { get; set; }

    /// <summary>
    /// Gets or sets the Title of the ToDoItem. This property is required.
    /// </summary>
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    public partial string? Title { get; set; }

    /// <summary>
    /// Gets or sets the Priority of the ToDoItem. The default value is Medium.
    /// </summary>
    [ObservableProperty]
    public partial Priority Priority { get; set; }

    /// <summary>
    /// Gets or sets the Description of the ToDoItem. This property is optional.
    /// </summary>
    [ObservableProperty]
    public partial string? Description { get; set; }

    /// <summary>
    /// Gets or sets the DueDate of the ToDoItem. The default value is 7 days from now.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentStatus))]
    public partial DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets the Progress of the ToDoItem. The default value is 0.
    /// </summary>
    /// <remarks>
    /// The value must be between 0 and 100.
    /// </remarks>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentStatus))]
    [NotifyDataErrorInfo]
    [Range(0, 100)]
    public partial int Progress { get; set; }
    
    /// <summary>
    /// Handles automatic completion date management when progress changes.
    /// Sets CompletedDate to the current time when progress reaches 100%
    /// and clears it when progress is reduced below 100%.
    /// </summary>
    /// <param name="value">The new progress value being set</param>
    partial void OnProgressChanged(int value)
    {
        // Automatically set the completion timestamp when an item is marked as done
        if (value >= 100)
        {
            this.CompletedDate = DateTime.Now;
        }
        else
        {
            // Clear the completion date when the item is no longer complete
            this.CompletedDate = null;
        }
    }


    /// <summary>
    /// Gets the calculated status of the ToDoItem based on progress and due date.
    /// Computed property that determines NotStarted, InProgress, Done, or Overdue status.
    /// Used by UI to display appropriate visual indicators and status information.
    /// </summary>
    public ToDoItemStatus CurrentStatus 
    {
        get
        {
            // Check for completion first as it has the highest priority
            if (Progress == 100)
                return ToDoItemStatus.Done;
            
            // Check for overdue status next
            if (DueDate < DateTime.Now)
                return ToDoItemStatus.Overdue;
            
            // Check if work has been started on the item
            if (Progress > 0)
                return ToDoItemStatus.InProgress;
            
            // Default to not started if none of the above conditions apply
            return ToDoItemStatus.NotStarted;
        }
    }
    
    /// <summary>
    /// Gets or sets the CreatedDate of the ToDoItem. The default value is now. This property is read-only.
    /// </summary>
    [ObservableProperty]
    public partial DateTime CreatedDate { get; private set; }

    /// <summary>
    /// Gets or sets the CompletedDate of the ToDoItem. As long as this property is null,
    /// the ToDoItem is not completed. This property is read-only.
    /// </summary>
    [ObservableProperty]
    public partial DateTime? CompletedDate { get; private set; }

    /// <summary>
    /// Command that updates progress and immediately saves to database.
    /// Used for quick progress updates from UI controls like sliders or buttons.
    /// Ensures data persistence for progress changes.
    /// </summary>
    /// <param name="value">The new progress value (0-100)</param>
    [RelayCommand]
    private async Task SetProgressAsync(int value)
    {
        Progress = value;
        await ToToDoItem().SaveAsync();
    }

    /// <summary>
    /// Converts this ViewModel back to the underlying ToDoItem model.
    /// Used for database persistence operations and data transfer.
    /// Includes conversion of the Category-ViewModel to the Category-Model.
    /// </summary>
    /// <returns>A new ToDoItem model populated with ViewModel data</returns>
    public ToDoItem ToToDoItem() => new ToDoItem()
    {
        Id = Id,
        Title = Title,
        Description = Description,
        CategoryId = Category.Id,
        Category = Category.ToCategory(),
        Progress = Progress,
        Priority = (int)Priority,
        DueDate = DueDate,
        CreatedDate = CreatedDate,
        CompletedDate = CompletedDate,
    };

    /// <summary>
    /// Creates a shallow copy of this ToDoItemViewModel.
    /// Used for creating edit copies without modifying the original.
    /// Implements ICloneable interface for standard cloning support.
    /// </summary>
    /// <returns>A shallow copy of this ViewModel instance</returns>
    public object Clone()
    {
        return MemberwiseClone();
    }

    /// <summary>
    /// Creates a typed shallow copy of this ToDoItemViewModel.
    /// Provides a convenience method for creating ToDoItemViewModel copies.
    /// Used when creating edit dialogs to preserve original data.
    /// </summary>
    /// <returns>A strongly typed shallow copy of this ViewModel</returns>
    public ToDoItemViewModel CloneToDoItemViewModel()
    { 
        return (ToDoItemViewModel)Clone();
    }
}