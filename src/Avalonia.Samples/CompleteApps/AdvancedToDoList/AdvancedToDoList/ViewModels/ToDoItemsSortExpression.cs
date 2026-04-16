using System;

namespace AdvancedToDoList.ViewModels;

/// <summary>
/// Represents a sorting expression for ToDoItems with a display name and sorting function.
/// Used to provide user-friendly sorting options in the UI with corresponding sort logic.
/// </summary>
/// <param name="sortExpression">Function that extracts a comparable value from a ToDoItemViewModel</param>
/// <param name="name">Display name shown to users in the UI</param>
public class ToDoItemsSortExpression(Func<ToDoItemViewModel, IComparable> sortExpression, string name)
{
    /// <summary>
    /// Sorts ToDoItems by their due date (ascending).
    /// Only the date portion is used for sorting, ignoring time.
    /// </summary>
    public static ToDoItemsSortExpression SortByDueDateExpression { get; } =
        new(x => x.DueDate.Date, "Due Date");
    
    /// <summary>
    /// Sorts ToDoItems alphabetically by title.
    /// Null or empty titles are sorted to the beginning.
    /// </summary>
    public static ToDoItemsSortExpression SortByTitleExpression { get; } =
        new(x => x.Title ?? string.Empty, "Title");

    /// <summary>
    /// Sorts ToDoItems by priority (ascending numeric, but inverted for priority levels).
    /// Higher priority items (lower enum values) appear first due to negation.
    /// </summary>
    public static ToDoItemsSortExpression SortByPriorityExpression { get; } =
        new(x => -((int)x.Priority), "Priority");

    /// <summary>
    /// Sorts ToDoItems alphabetically by category name.
    /// Uncategorized items appear first due to null handling.
    /// </summary>
    public static ToDoItemsSortExpression SortByCategoryExpression { get; } =
        new(x => x.Category.Name ?? string.Empty, "Category");

    /// <summary>
    /// Array of all available sorting expressions for UI binding.
    /// Used to populate sorting selection controls.
    /// </summary>
    public static ToDoItemsSortExpression[] AvailableSortExpressions { get; } =
    [
        SortByDueDateExpression,
        SortByTitleExpression,
        SortByPriorityExpression,
        SortByCategoryExpression
    ];
    
    /// <summary>
    /// Gets the display name shown to users in the UI.
    /// </summary>
    public string DisplayName { get; } = name;

    /// <summary>
    /// Gets the sorting function that extracts a comparable value from a ToDoItemViewModel.
    /// This function is used by DynamicData for efficient sorting operations.
    /// </summary>
    public Func<ToDoItemViewModel, IComparable> SortExpression { get; } = sortExpression;

    /// <summary>
    /// Returns the display name when converted to string.
    /// Useful for direct binding to UI controls that display the sort options.
    /// </summary>
    public override string ToString()
    {
        return DisplayName;
    }
}