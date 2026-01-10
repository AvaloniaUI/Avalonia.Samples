using AdvancedToDoList.Models;

namespace AdvancedToDoList.Helper;

public class DataBaseDto
{
    public Category[]? Categories { get; set; }
        
    public ToDoItem[]? ToDoItems { get; set; }
}