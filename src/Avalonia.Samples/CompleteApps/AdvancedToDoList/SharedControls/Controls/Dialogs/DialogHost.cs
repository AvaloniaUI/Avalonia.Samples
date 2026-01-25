using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace SharedControls.Controls;

public class DialogHost : ContentControl
{
    private IDisposable? _rootBoundsWatcher;

    protected override Type StyleKeyOverride => typeof(OverlayPopupHost);

    public DialogHost()
    {
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
    }
    
    protected override Size MeasureOverride(Size availableSize)
    {
        _ = base.MeasureOverride(availableSize);

        var topLevel = TopLevel.GetTopLevel(this);
        return topLevel?.ClientSize ?? default;
    }
    
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

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _rootBoundsWatcher?.Dispose();
        _rootBoundsWatcher = null;
    }
    
    private void OnRootBoundsChanged()
    {
        InvalidateMeasure();
    }

}