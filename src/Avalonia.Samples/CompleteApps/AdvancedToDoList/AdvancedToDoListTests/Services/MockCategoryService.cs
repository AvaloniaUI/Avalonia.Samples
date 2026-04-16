using AdvancedToDoList.Models;
using AdvancedToDoList.Services;

namespace AdvancedToDoListTests.Services;

/// <summary>
/// Mock implementation of ICategoryService for testing.
/// Controls save/delete outcomes and records what was saved.
/// Used to test EditCategoryViewModel's interaction with the category service.
/// </summary>
internal class MockCategoryService : ICategoryService
{
    /// <summary>
    /// Gets or sets whether the next save operation should succeed.
    /// Allows testing both successful and failed save scenarios.
    /// </summary>
    public bool SaveResult { get; set; } = true;

    /// <summary>
    /// Stores the category that was last saved.
    /// Used to verify the correct category was passed to the service.
    /// </summary>
    public Category? SavedCategory { get; private set; }

    /// <summary>
    /// Saves the category and records it for verification.
    /// </summary>
    /// <param name="category">The category to save</param>
    /// <returns>Task with the save result (true for success)</returns>
    public Task<bool> SaveCategoryAsync(Category category)
    {
        SavedCategory = category;
        return Task.FromResult(SaveResult);
    }

    /// <summary>
    /// Deletes the category (always succeeds in this mock).
    /// </summary>
    /// <param name="category">The category to delete</param>
    /// <returns>Task with the delete result (always true in this mock)</returns>
    public Task<bool> DeleteCategoryAsync(Category category)
    {
        return Task.FromResult(true);
    }
}