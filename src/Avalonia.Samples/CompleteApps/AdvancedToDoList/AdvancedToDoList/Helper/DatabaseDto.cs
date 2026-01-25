using AdvancedToDoList.Models;

namespace AdvancedToDoList.Helper;

/// <summary>
/// This is Data-Transfer-Object used to convert the Database from and to JSON-representation.
/// </summary>
/// <seealso href="https://en.wikipedia.org/wiki/Data_transfer_object"/>
public class DatabaseDto
{
    /// <summary>
    /// Gets or sets a collection of available Categories.
    /// </summary>
    public Category[]? Categories { get; set; }
        
    /// <summary>
    /// Gets or sets a collection of available ToDoItems.
    /// </summary>
    public ToDoItem[]? ToDoItems { get; set; }
}