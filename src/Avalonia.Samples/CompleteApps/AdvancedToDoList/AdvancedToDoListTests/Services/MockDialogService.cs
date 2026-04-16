using AdvancedToDoList.Services;
using SharedControls.Controls;

namespace AdvancedToDoListTests.Services;

/// <summary>
/// Mock implementation of IDialogService for testing.
/// Records dialog calls so we can verify behavior without actual UI interaction.
/// Used to test how EditToDoItemViewModel shows error dialogs and returns results.
/// </summary>
internal class MockDialogService : IDialogService
{
    /// <summary>
    /// Stores the title of the last dialog shown.
    /// Used to verify dialog titles match expectations.
    /// </summary>
    public string? LastTitle { get; private set; }

    /// <summary>
    /// Stores the content (message) of the last dialog shown.
    /// Used to verify dialog messages match expectations.
    /// </summary>
    public object? LastContent { get; private set; }

    /// <summary>
    /// Stores the result that was returned from the dialog.
    /// Used to verify dialog results match expectations.
    /// </summary>
    public object? ReturnedResult { get; private set; }

    /// <summary>
    /// Tracks whether ReturnResultFromOverlayDialog was called.
    /// Used to verify the dialog result flow was executed.
    /// </summary>
    public bool ReturnResultCalled { get; private set; }

    /// <summary>
    /// The result to return from the next dialog (defaults to default(T))
    /// Used to simulate different user responses in tests.
    /// </summary>
    public object? NextDialogResult { get; set; }

    /// <summary>
    /// Shows an overlay dialog and records the parameters.
    /// </summary>
    /// <param name="title">The dialog title</param>
    /// <param name="content">The dialog content/message</param>
    /// <param name="dialogCommands">The dialog commands available to the user</param>
    /// <returns>A task with the dialog result</returns>
    public Task<T?> ShowOverlayDialogAsync<T>(string title, object? content, params DialogCommand[] dialogCommands)
    {
        LastTitle = title;
        LastContent = content;
        var result = NextDialogResult switch
        {
            null => default,
            T typedResult => typedResult,
            _ => (T)NextDialogResult
        };
        return Task.FromResult(result);
    }

    /// <summary>
    /// Simulates returning a result from the dialog (e.g., user clicked OK).
    /// Used to complete the dialog flow during testing.
    /// </summary>
    /// <param name="result">The result to return from the dialog</param>
    public void ReturnResultFromOverlayDialog(object? result)
    {
        ReturnedResult = result;
        ReturnResultCalled = true;
    }
}