using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace SharedControls.Controls;

public class OverlayDialog : HeaderedContentControl
{
    private readonly TaskCompletionSource<object?> _tcs;

    public OverlayDialog(TaskCompletionSource<object?> tcs)
    {
        _tcs = tcs;
    }

    public OverlayDialog()
    {
        _tcs = new TaskCompletionSource<object?>();
    }

    /// <summary>
    /// Defines the <see cref="OverlayBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush> OverlayBrushProperty =
        AvaloniaProperty.Register<OverlayDialog, IBrush>(nameof(OverlayBrush), new SolidColorBrush(Colors.Black, 0.5));

    /// <summary>
    /// Gets or sets the OverlayBrush of the OverlayDialog.
    /// </summary>
    public IBrush OverlayBrush
    {
        get => GetValue(OverlayBrushProperty);
        set => SetValue(OverlayBrushProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="DialogCommands"/> property.
    /// </summary>
    public static readonly StyledProperty<DialogCommand[]> DialogCommandsProperty =
        AvaloniaProperty.Register<OverlayDialog, DialogCommand[]>(nameof(DialogCommands));

    /// <summary>
    /// Gets or sets the DialogCommands of the OverlayDialog.
    /// </summary>
    public DialogCommand[] DialogCommands
    {
        get => GetValue(DialogCommandsProperty);
        set => SetValue(DialogCommandsProperty, value);
    }

    /// <summary>
    /// Closes the dialog and returns the given result.
    /// </summary>
    /// <param name="result">the result to return</param>
    public void Close(object? result)
    {
        _tcs.SetResult(result);
    }

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