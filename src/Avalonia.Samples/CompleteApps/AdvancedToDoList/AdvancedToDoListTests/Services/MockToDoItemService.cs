using AdvancedToDoList.Models;
using AdvancedToDoList.Services;

namespace AdvancedToDoListTests.Services;

/// <summary>
/// Mock implementation of IToDoService for testing.
/// Controls save/delete outcomes and records what was saved.
/// Used to test EditToDoItemViewModel's interaction with the to-do service.
/// </summary>
internal class MockToDoItemService : IToDoItemService
{
    /// <summary>
    /// Gets or sets whether the next save operation should succeed.
    /// Allows testing both successful and failed save scenarios.
    /// </summary>
    public bool SaveResult { get; set; } = true;

    /// <summary>
    /// Stores the to-do item that was last saved.
    /// Used to verify the correct item was passed to the service.
    /// </summary>
    public ToDoItem? SavedItem { get; private set; }

    /// <summary>
    /// Saves the to-do item and records it for verification.
    /// </summary>
    /// <param name="item">The to-do item to save</param>
    /// <returns>Task with the save result (true for success)</returns>
    public Task<bool> SaveToDoItemAsync(ToDoItem item)
    {
        SavedItem = item;
        return Task.FromResult(SaveResult);
    }

    /// <summary>
    /// Deletes the to-do item (always succeeds in this mock).
    /// </summary>
    /// <param name="item">The to-do item to delete</param>
    /// <returns>Task with the delete result (always true in this mock)</returns>
    public Task<bool> DeleteToDoItemAsync(ToDoItem item)
    {
        return Task.FromResult(true);
    }
}