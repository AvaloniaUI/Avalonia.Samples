namespace SharedControls.Controls;

/// <summary>
/// Enumeration representing possible results from dialog interactions.
/// Used to communicate the user's choice back to the calling code.
/// Commonly returned from <see cref="DialogManager.ShowOverlayDialogAsync"/>, 
/// <see cref="DialogManager.ShowDialog"/>, and similar methods.
/// </summary>
/// <remarks>
/// Typical usage patterns:
/// - Confirmation dialogs (Yes/No/Cancel): <see cref="Yes"/>, <see cref="No"/>, <see cref="Cancel"/>
/// - Information dialogs (OK): <see cref="Ok"/>
/// - Cancelable dialogs (OK/Cancel): <see cref="Ok"/>, <see cref="Cancel"/>
/// - User dismissed dialog: <see cref="Cancel"/>
/// - No interaction or unhandled: <see cref="None"/>
/// </remarks>
/// <example>
/// Handling dialog results:
/// <code language="csharp">
/// <![CDATA[
/// var result = await this.ShowOverlayDialogAsync<DialogResult>(
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
/// ]]>
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