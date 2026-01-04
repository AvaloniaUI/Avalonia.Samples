using System;

namespace AdvancedToDoList.ViewModels;

public class ToDoItemsSortExpression(Func<ToDoItemViewModel, IComparable> sortExpression, string name)
{
    public static ToDoItemsSortExpression SortByDueDateExpression { get; } =
        new(x => x.DueDate.Date, "Due Date");
    
    public static ToDoItemsSortExpression SortByTitleExpression { get; } =
        new(x => x.Title ?? string.Empty, "Title");

    public static ToDoItemsSortExpression SortByPriorityExpression { get; } =
        new(x => -((int)x.Priority), "Priority");

    public static ToDoItemsSortExpression SortByCategoryExpression { get; } =
        new(x => x.Category.Name ?? string.Empty, "Category");

    public static ToDoItemsSortExpression[] AvailableSortExpressions { get; } =
    [
        SortByDueDateExpression,
        SortByTitleExpression,
        SortByPriorityExpression,
        SortByCategoryExpression
    ];
    
    public string DisplayName { get; } = name;

    public Func<ToDoItemViewModel, IComparable> SortExpression { get; } = sortExpression;

    public override string ToString()
    {
        return DisplayName;
    }
}