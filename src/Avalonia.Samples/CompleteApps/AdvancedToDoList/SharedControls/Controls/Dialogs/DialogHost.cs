using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace SharedControls.Controls;

/// <summary>
/// A specialized content control used to host and center dialogs within an application.
/// 
/// This control automatically centers itself in the window and ensures it resizes
/// properly when the window changes size — important because it's often hosted on an
/// OverlayLayer (which is implemented as a Canvas and doesn't automatically forward resize events).
/// 
/// Use this as the container for dialogs to ensure they stay centered and responsive.
/// </summary>
public class DialogHost : ContentControl
{
    /// <summary>
    /// Keeps track of the event subscription that listens for window size changes.
    /// When the window bounds change, this watcher triggers a measure update
    /// so the dialog stays properly centered and sized.
    /// 
    /// A disposable object — always dispose when the control is detached to avoid memory leaks.
    /// </summary>
    private IDisposable? _rootBoundsWatcher;

    /// <summary>
    /// Overrides the default style key to use the OverlayPopupHost style.
    /// This tells Avalonia to apply the correct visual appearance (including popup behavior)
    /// to this control.
    /// </summary>
    protected override Type StyleKeyOverride => typeof(OverlayPopupHost);

    /// <summary>
    /// Creates a new instance of DialogHost and sets the default alignment.
    /// Centers the dialog horizontally and vertically within its parent container.
    /// </summary>
    public DialogHost()
    {
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
    }
    
    /// <summary>
    /// Calculates the size this control should be based on available space.
    /// 
    /// In this implementation, the control always tries to match the size of the top-level window
    /// (e.g., the main window). This ensures the dialog fills the window area it's hosted in.
    /// 
    /// Note: We call base.MeasureOverride first to respect any inherited measurement logic,
    /// but then override the result with the window's client size.
    /// </summary>
    /// <param name="availableSize">The space available from the parent layout system</param>
    /// <returns>The desired size (set to the window's client size)</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        _ = base.MeasureOverride(availableSize);

        var topLevel = TopLevel.GetTopLevel(this);
        return topLevel?.ClientSize ?? default;
    }
    
    /// <summary>
    /// Called when this control is added to the visual tree.
    /// 
    /// Sets up a listener for window resizing. Since DialogHost is typically hosted
    /// on an OverlayLayer (a Canvas), normal resize events won't fire automatically.
    /// This subscription ensures the dialog recalculates its size whenever the window changes.
    /// </summary>
    /// <param name="e">Event arguments containing information about the visual tree attachment</param>
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
    
        if (e.Root is Control root)
        {
            // OverlayLayer is a Canvas, so we won't get a signal to resize if the window
            // bounds change. Subscribe to force update
            _rootBoundsWatcher = root.GetObservable(BoundsProperty).Subscribe(_ => OnRootBoundsChanged());
        }
        InvalidateMeasure();
    }

    /// <summary>
    /// Called when this control is removed from the visual tree.
    /// 
    /// Cleans up the resize watcher subscription to prevent memory leaks.
    /// Disposing the watcher stops it from holding onto resources after
    /// the control is no longer in use.
    /// </summary>
    /// <param name="e">Event arguments containing information about the visual tree detachment</param>
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _rootBoundsWatcher?.Dispose();
        _rootBoundsWatcher = null;
    }
    
    /// <summary>
    /// Updates the layout measurement when the window bounds have changed.
    /// 
    /// Called automatically when the window resizes, this method triggers a new
    /// measurement pass so the dialog updates its size and position accordingly.
    /// </summary>
    private void OnRootBoundsChanged()
    {
        InvalidateMeasure();
    }

}