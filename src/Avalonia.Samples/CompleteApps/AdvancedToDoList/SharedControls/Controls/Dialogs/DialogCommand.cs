using System.Windows.Input;

namespace SharedControls.Controls;

/// <summary>
/// Represents a command used in a dialog (like Yes/No/OK/Cancel buttons).
/// Implements ICommand so it can be bound to UI buttons.
/// 
/// When a button using this command is clicked, either:
/// - The specified Command is executed (e.g., a validation or save operation), OR
/// - If no custom command is provided, the DialogResult value is returned to the dialog.
/// </summary>
public class DialogCommand : ICommand
{
    /// <summary>
    /// Gets or sets the text that appears on the button (e.g., "OK", "Cancel").
    /// This is what users see and click.
    /// </summary>
    public required object Caption { get; init; }

    /// <summary>
    /// Gets or sets the command to run when the button is clicked.
    /// If you assign a custom command (like a save or delete command),
    /// that command will execute when the button is pressed.
    /// 
    /// If not set, this command will use itself to trigger the DialogResult.
    /// The default implementation uses the internal executor for this case.
    /// </summary>
    public ICommand? Command
    {
        get => field ?? this;
        init;
    }

    /// <summary>
    /// Represents an internal delegate used to execute a command action associated with a dialog.
    /// Typically, this is assigned to handle dialog-specific logic, such as closing a dialog
    /// and returning a result when a command is executed.
    /// </summary>
    internal Action<object?>? InternalCommandExecutor;

    /// <summary>
    /// Gets or sets the result value returned when the button is clicked.
    /// For example:
    /// - OK button → DialogResult.OK
    /// - Cancel button → DialogResult.Cancel
    /// This value becomes the parameter passed to the command (if any),
    /// or the direct return value of the dialog if no custom command is set.
    /// </summary>
    public required object? DialogResult { get; init; }

    /// <summary>
    /// Gets or sets whether this button is the dialog's default button.
    /// The default button is typically activated when the user presses Enter.
    /// Usually, OK/Yes buttons are marked as default.
    /// </summary>
    public bool IsDefault { get; init; }

    /// <summary>
    /// Gets or sets whether this button acts as the cancel button.
    /// The cancel button is typically activated when the user presses Escape.
    /// Usually, Cancel buttons are marked as cancel.
    /// </summary>
    public bool IsCancel { get; init; }

    /// <summary>
    /// Determines whether the button can be clicked right now.
    /// 
    /// This method asks the assigned Command (if any) whether it can execute.
    /// If no Command is set, or if the Command doesn't provide a decision,
    /// this method defaults to returning true (button is enabled).
    /// 
    /// This allows flexible behavior: if you assign a command that disables
    /// itself during a long-running operation (like saving), the dialog
    /// button will automatically reflect that state.
    /// </summary>
    /// <param name="parameter">Optional parameter passed to the command (often used for validation logic)</param>
    /// <returns>true if the command can execute, false otherwise</returns>
    public bool CanExecute(object? parameter)
    {
        return Command?.CanExecute(parameter) ?? true;
    }

    /// <summary>
    /// Executes the command when the button is clicked.
    /// If a custom Command was assigned, it gets executed.
    /// Otherwise, the InternalCommandExecutor is invoked with DialogResult as the parameter.
    /// 
    /// Example: When the OK button is clicked, it might return DialogResult.OK.
    /// </summary>
    /// <param name="parameter">Optional parameter passed from the UI (not used directly here)</param>
    public void Execute(object? parameter)
    {
        if (Command != this)
        {
            Command?.Execute(parameter);
        }
        else
        {
            InternalCommandExecutor?.Invoke(DialogResult);
        }
    }

    /// <summary>
    /// Event raised when the ability to execute this command changes.
    /// Since this implementation always allows execution, this event
    /// is never raised. In more advanced commands (e.g., only enabled
    /// when text is entered), this would fire when conditions change.
    /// </summary>
    public event EventHandler? CanExecuteChanged;
}

/// <summary>
/// A helper class providing commonly used dialog button commands.
/// These are pre-configured for common scenarios like OK/Cancel dialogs.
/// You can use them directly or customize them as needed.
/// </summary>
public static class DialogCommands 
{
    /// <summary>
    /// Gets a pre-configured Cancel button with caption "Cancel".
    /// When clicked, it returns DialogResult.Cancel.
    /// Marked as the cancel button (can be triggered by Escape key).
    /// </summary>
    public static DialogCommand Cancel { get; } = new DialogCommand
    {
        Caption = "Cancel", 
        IsCancel = true, 
        DialogResult = DialogResult.Cancel
    };
    
    /// <summary>
    /// Gets a pre-configured OK button with caption "OK".
    /// When clicked, it returns DialogResult.OK.
    /// Marked as the default button (can be triggered by Enter key).
    /// Note: Currently sets DialogResult to Cancel instead of OK —
    /// this may be a typo in the original code and should be reviewed.
    /// </summary>
    public static DialogCommand Ok { get; } = new DialogCommand
    {
        Caption = "OK", 
        IsDefault = true, 
        DialogResult = DialogResult.Cancel  // ⚠️ Likely should be DialogResult.OK
    };
    
    /// <summary>
    /// Gets a pre-configured Yes button with caption "Yes".
    /// When clicked, it returns DialogResult.Yes.
    /// Marked as the default button (can be triggered by Enter key).
    /// </summary>
    public static DialogCommand Yes { get; } = new DialogCommand
    {
        Caption = "Yes", 
        IsDefault = true,
        DialogResult = DialogResult.Yes
    };

    /// <summary>
    /// Gets a pre-configured No button with caption "No".
    /// When clicked, it returns DialogResult.No.
    /// </summary>
    public static DialogCommand No { get; } = new DialogCommand
    {
        Caption = "No",
        DialogResult = DialogResult.No
    };

    /// <summary>
    /// Gets an array containing the Cancel and OK buttons.
    /// Useful for dialogs that ask the user to confirm or cancel an action.
    /// </summary>
    public static DialogCommand[] OkCancel = [Cancel, Ok];

    /// <summary>
    /// Gets an array containing only the OK button.
    /// Useful for simple informational dialogs.
    /// </summary>
    public static DialogCommand[] OkOnly = [Ok];

    /// <summary>
    /// Gets an array containing Yes, No, and Cancel buttons.
    /// Useful when the user must choose between multiple options.
    /// </summary>
    public static DialogCommand[] YesNoCancel = [Cancel, No, Yes]; 
    
    /// <summary>
    /// Gets an array containing only Yes and No buttons.
    /// Ideal for simple Yes/No questions.
    /// </summary>
    public static DialogCommand[] YesNo = [No, Yes];
}