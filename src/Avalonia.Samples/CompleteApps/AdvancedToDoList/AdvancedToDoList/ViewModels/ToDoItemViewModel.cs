using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AdvancedToDoList.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdvancedToDoList.ViewModels;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
[UnconditionalSuppressMessage("Trimming",
    "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
    Justification = "All properties accessed by reflection are also otherwise used, so they will not be trimmed.")]
[UnconditionalSuppressMessage("Trimming",
    "IL2112: 'DynamicallyAccessedMembersAttribute' on 'AdvancedToDoList.ViewModels.ToDoItemViewModel' or one of its base types references 'AdvancedToDoList.ViewModels.ToDoItemViewModel.Title.set' which requires unreferenced code. The type of the current instance cannot be statically discovered.",
    Justification = "Made sure the needed items are not trimmed.")]
public partial class ToDoItemViewModel : ViewModelBase, ICloneable
{
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
    public partial int? Id { get; private set; }

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
    
    partial void OnProgressChanged(int value)
    {
        // Store the completed date if the progress is 100
        if (value >= 100)
        {
            this.CompletedDate = DateTime.Now;
        }
        else
        {
            this.CompletedDate = null;
        }
    }


    public ToDoItemStatus CurrentStatus 
    {
        get
        {
            if (Progress == 100)
                return ToDoItemStatus.Done;
            
            if (DueDate < DateTime.Now)
                return ToDoItemStatus.Overdue;
            
            if (Progress > 0)
                return ToDoItemStatus.InProgress;
            
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

    [RelayCommand]
    private async Task SetProgressAsync(int value)
    {
        Progress = value;
        await ToToDoItem().SaveAsync();
    }

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

    public object Clone()
    {
        return MemberwiseClone();
    }

    public ToDoItemViewModel CloneToDoItemViewModel()
    { 
        return (ToDoItemViewModel)Clone();
    }
}