using System;
using System.ComponentModel.DataAnnotations;
using AdvancedToDoList.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AdvancedToDoList.ViewModels;

public partial class ToDoItemViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the Id of the ToDoItem. This property is read-only.
    /// </summary>
    [ObservableProperty]
    public partial int? Id { get; private set; }

    /// <summary>
    /// Gets or sets the Category of the ToDoItem.
    /// </summary>
    [ObservableProperty]
    public partial CategoryViewModel? Category { get; set; }

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
    public partial Priority Priority { get; set; } = Priority.Medium;

    /// <summary>
    /// Gets or sets the Description of the ToDoItem. This property is optional.
    /// </summary>
    [ObservableProperty]
    public partial string? Description { get; set; }

    /// <summary>
    /// Gets or sets the DueDate of the ToDoItem. The default value is 7 days from now.
    /// </summary>
    [ObservableProperty]
    public partial DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);

    /// <summary>
    /// Gets or sets the Progress of the ToDoItem. The default value is 0.
    /// </summary>
    /// <remarks>
    /// The value must be between 0 and 100.
    /// </remarks>
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(0, 100)]
    public partial int Progress { get; set; } = 0;

    /// <summary>
    /// Gets or sets the CreatedDate of the ToDoItem. The default value is now. This property is read-only.
    /// </summary>
    [ObservableProperty]
    public partial DateTime CreatedDate { get; private set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the CompletedDate of the ToDoItem. As long as this property is null,
    /// the ToDoItem is not completed. This property is read-only.
    /// </summary>
    [ObservableProperty]
    public partial DateTime? CompletedDate { get; private set; }
}