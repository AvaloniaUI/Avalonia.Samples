using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace SharedControls.Controls;

[TemplatePart (nameof(PART_SplitView), typeof(SplitView), IsRequired = true)]
public class HamburgerMenu : HeaderedContentControl
{
    private SplitView? PART_SplitView;
    
    public static readonly DirectProperty<HamburgerMenu, IList<IHamburgerMenuItem>> MenuItemsProperty =
        AvaloniaProperty.RegisterDirect<HamburgerMenu, IList<IHamburgerMenuItem>>(
            nameof(MenuItems), 
            o => o.MenuItems, 
            (o, v) => o.MenuItems = v);

    public IList<IHamburgerMenuItem> MenuItems
    {
        get;
        set => SetAndRaise(MenuItemsProperty, ref field, value);
    } = new ObservableCollection<IHamburgerMenuItem>();

    public static readonly DirectProperty<HamburgerMenu, IList<IHamburgerMenuItem>> OptionsMenuItemsProperty = 
        AvaloniaProperty.RegisterDirect<HamburgerMenu, IList<IHamburgerMenuItem>>(
            nameof(OptionsMenuItems), 
            o => o.OptionsMenuItems, 
            (o, v) => o.OptionsMenuItems = v);

    public IList<IHamburgerMenuItem> OptionsMenuItems
    {
        get;
        set => SetAndRaise(OptionsMenuItemsProperty, ref field, value);
    } = new ObservableCollection<IHamburgerMenuItem>();


    public static readonly DirectProperty<HamburgerMenu, IHamburgerMenuItem?> SelectedMenuItemProperty 
        = AvaloniaProperty.RegisterDirect<HamburgerMenu, IHamburgerMenuItem?>(
            nameof(SelectedMenuItem), 
            o => o.SelectedMenuItem, 
            (o, v) =>
            {
                if (v != null)
                    o.SelectedMenuItem = v;
            });
    
    public IHamburgerMenuItem? SelectedMenuItem
    {
        get;
        set => SetAndRaise(SelectedMenuItemProperty, ref field, value);
    }

    public static readonly StyledProperty<bool> IsPaneOpenProperty = 
        SplitView.IsPaneOpenProperty.AddOwner<HamburgerMenu>();

    public bool IsPaneOpen
    {
        get => GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }
    
    public static readonly StyledProperty<IBrush?> PaneBackgroundProperty =
        SplitView.PaneBackgroundProperty.AddOwner<HamburgerMenu>();

    public IBrush? PaneBackground
    {
        get => GetValue(PaneBackgroundProperty);
        set => SetValue(PaneBackgroundProperty, value);
    }
    
    public static readonly StyledProperty<double> CompactPaneLengthProperty =
        SplitView.CompactPaneLengthProperty.AddOwner<HamburgerMenu>();

    public double CompactPaneLength
    {
        get => GetValue(CompactPaneLengthProperty);
        set => SetValue(CompactPaneLengthProperty, value);
    }
    
    public static readonly StyledProperty<double> OpenPaneLengthProperty =
        SplitView.OpenPaneLengthProperty.AddOwner<HamburgerMenu>();

    public double OpenPaneLength
    {
        get => GetValue(OpenPaneLengthProperty);
        set => SetValue(OpenPaneLengthProperty, value);
    }

    public static readonly StyledProperty<double> AutoClosePaneThresholdProperty = AvaloniaProperty.Register<HamburgerMenu, double>(
        nameof(AutoClosePaneThreshold), 600);

    public double AutoClosePaneThreshold
    {
        get => GetValue(AutoClosePaneThresholdProperty);
        set => SetValue(AutoClosePaneThresholdProperty, value);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        AutoCollapsePane();
        SelectedMenuItem ??= MenuItems.FirstOrDefault();
        
        base.OnLoaded(e);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        PART_SplitView = e.NameScope.Find<SplitView>("PART_SplitView");
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsPaneOpenProperty && !_isAutoCollapsing)
        {
            _wasPaneOpenBeforeAutoCollapse = IsPaneOpen;
        }
        
        if (change.Property == AutoClosePaneThresholdProperty 
            || change.Property == SelectedMenuItemProperty 
            || change.Property == BoundsProperty)
        {
            AutoCollapsePane();
        }

        if (change.Property == AutoClosePaneThresholdProperty || change.Property == BoundsProperty)
        {
            PART_SplitView?.DisplayMode = (AutoClosePaneThreshold > 0 && Bounds.Width < AutoClosePaneThreshold) 
                ? SplitViewDisplayMode.CompactOverlay 
                : SplitViewDisplayMode.CompactInline;
        }

        if (change.Property == MenuItemsProperty)
        {
            if (change.OldValue is INotifyCollectionChanged oldMenuItemsCollection)
            {
                oldMenuItemsCollection.CollectionChanged -= MenuItemsCollectionOnCollectionChanged;
            }
            
            SelectedMenuItem = MenuItems.FirstOrDefault();

            if (change.NewValue is INotifyCollectionChanged newMenuItemsCollection)
            {
                newMenuItemsCollection.CollectionChanged += MenuItemsCollectionOnCollectionChanged;
            }
        }
    }

    private void MenuItemsCollectionOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SelectedMenuItem ??= MenuItems.FirstOrDefault();
    }

    private bool _wasPaneOpenBeforeAutoCollapse;
    private bool _isAutoCollapsing;
    
    private void AutoCollapsePane()
    {
        if (_wasPaneOpenBeforeAutoCollapse && Bounds.Width >= AutoClosePaneThreshold)
        {
            IsPaneOpen = true;
        } 
        
        if (IsLoaded && IsPaneOpen && AutoClosePaneThreshold > 0 && Bounds.Width < AutoClosePaneThreshold)
        {
            _isAutoCollapsing = true;
            IsPaneOpen = false;
            _isAutoCollapsing = false;
        }
    }
}