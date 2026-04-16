using AdvancedToDoList.Models;

namespace AdvancedToDoList.Helper;

/// <summary>
/// Data Transfer Object (DTO) used for database import/export operations.
/// Provides a structured format for converting database data to/from JSON representation.
/// Used in data backup/restore and migration scenarios.
/// </summary>
/// <seealso href="https://en.wikipedia.org/wiki/Data_transfer_object"/>
public class DatabaseDto
{
    /// <summary>
    /// Gets or sets the collection of categories to be imported/exported.
    /// Null values indicate no categories are present in the data transfer.
    /// Used during database backup/restore operations to preserve category structure.
    /// </summary>
    public Category[]? Categories { get; set; }
        
    /// <summary>
    /// Gets or sets the collection of ToDoItems to be imported/exported.
    /// Null values indicate no ToDoItems are present in the data transfer.
    /// Maintains all ToDoItem data including relationships to categories.
    /// </summary>
    public ToDoItem[]? ToDoItems { get; set; }
}