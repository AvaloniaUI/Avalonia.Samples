using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;

namespace SharedControls.Controls;

/// <summary>
/// A dialog control that displays content over the main application interface.
/// 
/// This control provides:
/// - An optional semi-transparent overlay background (black with 50% opacity by default)
/// - A header area (inherited from HeaderedContentControl)
/// - Dialog command buttons (OK, Cancel, Yes/No, etc.)
/// - Keyboard shortcuts (Enter = default button, Escape = cancel button)
/// 
/// It works with the TaskCompletionSource to support await/async dialog patterns,
/// allowing callers to wait for user interaction before continuing.
/// </summary>
public class OverlayDialog : HeaderedContentControl
{
    /// <summary>
    /// The task completion source used to signal when the dialog is closed.
    /// 
    /// The dialog's closing operation is linked to this object — when Close() is called,
    /// it completes the task with the specified result, allowing the awaiter to continue.
    /// </summary>
    private readonly TaskCompletionSource<object?> _tcs;

    /// <summary>
    /// Creates a new OverlayDialog and uses the provided task completion source.
    /// 
    /// Called internally by the DialogHost or when a dialog needs to be tied to
    /// an existing awaitable task (e.g., when starting an asynchronous dialog operation).
    /// </summary>
    /// <param name="tcs">The task completion source to use for signaling dialog closure</param>
    public OverlayDialog(TaskCompletionSource<object?> tcs)
    {
        _tcs = tcs;
    }

    /// <summary>
    /// Creates a new OverlayDialog and creates its own task completion source.
    /// 
    /// This is useful when creating the dialog directly without external coordination.
    /// The dialog can still be awaited by accessing the Task property on the TCS.
    /// </summary>
    public OverlayDialog()
    {
        _tcs = new TaskCompletionSource<object?>();
    }

    /// <summary>
    /// Defines the <see cref="OverlayBrush"/> property.
    /// 
    /// This is the backing Avalonia property for the semi-transparent background
    /// that covers the main application content behind the dialog.
    /// The default brush is black with 50% opacity.
    /// </summary>
    public static readonly StyledProperty<IBrush> OverlayBrushProperty =
        AvaloniaProperty.Register<OverlayDialog, IBrush>(nameof(OverlayBrush), new SolidColorBrush(Colors.Black, 0.5));

    /// <summary>
    /// Gets or sets the brush used to draw the overlay background.
    /// 
    /// This fills the area behind the dialog content, dimming the main UI
    /// to draw attention to the dialog. You can customize the appearance
    /// by setting a different brush (e.g., different color or opacity).
    /// </summary>
    public IBrush OverlayBrush
    {
        get => GetValue(OverlayBrushProperty);
        set => SetValue(OverlayBrushProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="DialogCommands"/> property.
    /// 
    /// This property holds an array of DialogCommand objects that define
    /// the available buttons (like OK, Cancel, Yes, No) for this dialog.
    /// </summary>
    public static readonly StyledProperty<DialogCommand[]> DialogCommandsProperty =
        AvaloniaProperty.Register<OverlayDialog, DialogCommand[]>(nameof(DialogCommands));

    /// <summary>
    /// Gets or sets the array of dialog commands that appear as buttons.
    /// 
    /// Each command in this array typically renders as a button in the dialog's
    /// footer. The command's Caption becomes the button text, and clicking it
    /// executes the command, which may close the dialog with a specific result.
    /// </summary>
    public DialogCommand[] DialogCommands
    {
        get => GetValue(DialogCommandsProperty);
        set => SetValue(DialogCommandsProperty, value);
    }

    /// <summary>
    /// Closes the dialog and returns the specified result to the awaiter.
    /// 
    /// When the user clicks a button (or presses Enter/Escape), this method is called
    /// to complete the dialog's associated task with the given result value.
    /// 
    /// Example: Clicking "Cancel" might call Close(DialogResult.Cancel).
    /// </summary>
    /// <param name="result">The value to return when the dialog completes (e.g., DialogResult.OK)</param>
    public void Close(object? result)
    {
        _tcs.SetResult(result);
    }

    /// <summary>
    /// Handles keyboard input at the dialog level.
    /// 
    /// This method enables standard dialog keyboard shortcuts:
    /// - Pressing Escape triggers the first command with IsCancel = true
    /// - Pressing Enter triggers the first command with IsDefault = true
    /// 
    /// This provides a familiar experience where users can confirm/cancel dialogs
    /// without using the mouse.
    /// </summary>
    /// <param name="e">Contains information about the key that was pressed</param>
    protected override void OnKeyUp(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Escape:
                DialogCommands.SingleOrDefault(x => x.IsCancel)?.Execute(null);
                break;
            case Key.Enter:
                DialogCommands.SingleOrDefault(x => x.IsDefault)?.Execute(null);
                break;
        }

        e.Handled = true;
        base.OnKeyUp(e);
    }
    
    /// <summary>
    /// Responds to changes in any Avalonia property, especially DialogCommands.
    /// 
    /// When the DialogCommands property is set (e.g., when the dialog is initialized),
    /// this method wires up internal command executors so that each DialogCommand
    /// can close the dialog with the appropriate result when clicked.
    /// 
    /// Without this wiring, the command buttons would appear but not actually close
    /// the dialog — the internal executor links the command execution to the Close() method.
    /// </summary>
    /// <param name="change">Contains details about which property changed and its old/new values</param>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        // Add internal command executors to the dialog commands. Otherwise, the buttons won't work.
        if (change.Property == DialogCommandsProperty)
        {
            foreach (var command in DialogCommands)
            {
                command.InternalCommandExecutor = Close;
            }
        }
    }
}