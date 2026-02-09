namespace SharedControls.Controls;

/// <summary>
/// Enumeration representing possible results from dialog interactions.
/// Used to communicate user's choice back to calling code.
/// Commonly returned from ShowDialog, ShowOverlayDialog, and similar dialog methods.
/// </summary>
/// <remarks>
/// Typical usage patterns:
/// - Confirmation dialogs (Yes/No/Cancel): DialogResult.Yes, DialogResult.No, DialogResult.Cancel
/// - Information dialogs (OK): DialogResult.Ok
/// - Cancelable dialogs (OK/Cancel): DialogResult.Ok, DialogResult.Cancel
/// - User dismissed dialog: DialogResult.Cancel
/// - Unhandled or dismissed: DialogResult.None
/// </remarks>
/// <example>
/// <code>
/// var result = await this.ShowOverlayDialogAsync&lt;DialogResult&gt;(
///     "Confirm Delete",
///     "Are you sure you want to delete?",
///     DialogCommands.YesNoCancel);
/// 
/// if (result == DialogResult.Yes)
/// {
///     // User confirmed - proceed with deletion
///     DeleteItem();
/// }
/// else if (result == DialogResult.No || result == DialogResult.Cancel)
/// {
///     // User declined - cancel operation
///     return;
/// }
/// </code>
/// </example>
public enum DialogResult
{
    /// <summary>
    /// No result or user dismissed dialog without selecting an option.
    /// May indicate dialog was closed by clicking outside or pressing Escape.
    /// </summary>
    None,
    
    /// <summary>
    /// User chose "Yes" option in a confirmation dialog.
    /// Typically indicates affirmative action should be taken.
    /// Commonly used with "Are you sure?" type prompts.
    /// </summary>
    Yes,
    
    /// <summary>
    /// User chose "No" option in a confirmation dialog.
    /// Typically indicates negative response to a question or prompt.
    /// Commonly used with "Are you sure?" type prompts.
    /// </summary>
    No,
    
    /// <summary>
    /// User chose "OK" option in an information or prompt dialog.
    /// Indicates acknowledgement or confirmation of presented information.
    /// Used in single-choice dialogs where user just acknowledges content.
    /// </summary>
    Ok,
    
    /// <summary>
    /// User chose "Cancel" option or dismissed the dialog.
    /// Indicates operation should be aborted or no action taken.
    /// Often the result of Escape key, Cancel button, or dialog dismissal.
    /// </summary>
    Cancel
}