using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace SharedControls.Controls;

/// <summary>
/// A user interface control that provides a hamburger-style navigation menu,
/// commonly used in mobile or compact applications. It uses an Avalonia SplitView
/// to show/hide a side panel (pane) with navigation items.
/// </summary>
/// <example>
/// In your XAML, you would declare it like:
/// <![CDATA[
/// <controls:HamburgerMenu MenuItems="{Binding MainItems}"
///                         OptionsMenuItems="{Binding Options}" />
/// ]]>
/// </example>
[TemplatePart (nameof(PART_SplitView), typeof(SplitView), IsRequired = true)]
public class HamburgerMenu : HeaderedContentControl
{
    /// <summary>
    /// Initializes a new instance of the HamburgerMenu class.
    /// Sets up empty collections for menu items and options.
    /// </summary>
    public HamburgerMenu()
    {
        MenuItems = new ObservableCollection<IHamburgerMenuItem>();
        OptionsMenuItems = new ObservableCollection<IHamburgerMenuItem>();
    }
  
    /// <summary>
    /// Reference to the SplitView control defined in the control template.
    /// This is auto-assigned when the template is applied (via OnApplyTemplate).
    /// </summary>
    private SplitView? PART_SplitView;
  
    /// <summary>
    /// Defines the dependency property for the main navigation menu items.
    /// This allows binding and reactive updates to the menu.
    /// </summary>
    public static readonly DirectProperty<HamburgerMenu, IList<IHamburgerMenuItem>> MenuItemsProperty =
        AvaloniaProperty.RegisterDirect<HamburgerMenu, IList<IHamburgerMenuItem>>(
            nameof(MenuItems), 
            o => o.MenuItems, 
            (o, v) => o.MenuItems = v);

    /// <summary>
    /// Gets or sets the collection of main menu items displayed in the navigation pane.
    /// These typically include primary navigation destinations.
    /// </summary>
    public IList<IHamburgerMenuItem> MenuItems
    {
        get;
        set => SetAndRaise(MenuItemsProperty, ref field, value);
    }

    /// <summary>
    /// Defines the dependency property for the options menu items (usually at the bottom).
    /// </summary>
    public static readonly DirectProperty<HamburgerMenu, IList<IHamburgerMenuItem>> OptionsMenuItemsProperty = 
        AvaloniaProperty.RegisterDirect<HamburgerMenu, IList<IHamburgerMenuItem>>(
            nameof(OptionsMenuItems), 
            o => o.OptionsMenuItems, 
            (o, v) => o.OptionsMenuItems = v);

    /// <summary>
    /// Gets or sets the collection of optional menu items (often shown at the bottom of the pane).
    /// These might include settings, help, or less frequently used actions.
    /// </summary>
    public IList<IHamburgerMenuItem> OptionsMenuItems
    {
        get;
        set => SetAndRaise(OptionsMenuItemsProperty, ref field, value);
    }

    /// <summary>
    /// Defines the dependency property for tracking the currently selected menu item.
    /// </summary>
    public static readonly DirectProperty<HamburgerMenu, IHamburgerMenuItem?> SelectedMenuItemProperty 
        = AvaloniaProperty.RegisterDirect<HamburgerMenu, IHamburgerMenuItem?>(
            nameof(SelectedMenuItem), 
            o => o.SelectedMenuItem, 
            (o, v) =>
            {
                if (v != null)
                    o.SelectedMenuItem = v;
            });
  
    /// <summary>
    /// Gets or sets the currently selected menu item.
    /// When changed, it triggers selection logic and can be used to show the corresponding content.
    /// </summary>
    public IHamburgerMenuItem? SelectedMenuItem
    {
        get;
        set => SetAndRaise(SelectedMenuItemProperty, ref field, value);
    }

    /// <summary>
    /// Defines the dependency property for whether the navigation pane is open.
    /// true means the menu is visible; false means it's collapsed.
    /// </summary>
    public static readonly StyledProperty<bool> IsPaneOpenProperty = 
        SplitView.IsPaneOpenProperty.AddOwner<HamburgerMenu>(new StyledPropertyMetadata<bool>(true));

    /// <summary>
    /// Gets or sets a value indicating whether the navigation pane is currently open.
    /// </summary>
    public bool IsPaneOpen
    {
        get => GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }
  
    /// <summary>
    /// Defines the dependency property for the background color of the navigation pane.
    /// </summary>
    public static readonly StyledProperty<IBrush?> PaneBackgroundProperty =
        SplitView.PaneBackgroundProperty.AddOwner<HamburgerMenu>();

    /// <summary>
    /// Gets or sets the background brush of the navigation pane.
    /// This controls the color or pattern behind the menu items.
    /// </summary>
    public IBrush? PaneBackground
    {
        get => GetValue(PaneBackgroundProperty);
        set => SetValue(PaneBackgroundProperty, value);
    }
  
    /// <summary>
    /// Defines the dependency property for the width of the pane when it's in compact (closed) mode.
    /// </summary>
    public static readonly StyledProperty<double> CompactPaneLengthProperty =
        SplitView.CompactPaneLengthProperty.AddOwner<HamburgerMenu>();

    /// <summary>
    /// Gets or sets the width of the navigation pane when it is closed/miniaturized.
    /// </summary>
    public double CompactPaneLength
    {
        get => GetValue(CompactPaneLengthProperty);
        set => SetValue(CompactPaneLengthProperty, value);
    }
  
    /// <summary>
    /// Defines the dependency property for the width of the pane when fully open.
    /// </summary>
    public static readonly StyledProperty<double> OpenPaneLengthProperty =
        SplitView.OpenPaneLengthProperty.AddOwner<HamburgerMenu>();

    /// <summary>
    /// Gets or sets the full width of the navigation pane when it's open.
    /// </summary>
    public double OpenPaneLength
    {
        get => GetValue(OpenPaneLengthProperty);
        set => SetValue(OpenPaneLengthProperty, value);
    }

    /// <summary>
    /// Defines the dependency property that determines when the pane should automatically close.
    /// If the application width drops below this value, the pane closes automatically.
    /// </summary>
    public static readonly StyledProperty<double> AutoClosePaneThresholdProperty = AvaloniaProperty.Register<HamburgerMenu, double>(
        nameof(AutoClosePaneThreshold), 600);

    /// <summary>
    /// Gets or sets the width threshold (in device-independent units) at which the navigation pane
    /// automatically collapses. If the window is narrower than this value, the pane will hide.
    /// Set to 0 or less to disable auto-closing behavior.
    /// </summary>
    public double AutoClosePaneThreshold
    {
        get => GetValue(AutoClosePaneThresholdProperty);
        set => SetValue(AutoClosePaneThresholdProperty, value);
    }

    /// <summary>
    /// Called when the control is loaded (after visual tree is ready).
    /// Sets up initial pane state based on window size.
    /// </summary>
    protected override void OnLoaded(RoutedEventArgs e)
    {
        AutoCollapsePane();
        base.OnLoaded(e);
    }

    /// <summary>
    /// Called when the control template is applied.
    /// Finds and stores a reference to the SplitView named "PART_SplitView" from the template.
    /// </summary>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        PART_SplitView = e.NameScope.Find<SplitView>("PART_SplitView");
    }

    /// <summary>
    /// Handles property changes to manage pane behavior and item selection.
    /// For example:
    /// - Toggles auto-closing when pane opens/closes
    /// - Updates auto-close behavior when window size changes
    /// - Handles adding/removing menu items dynamically
    /// </summary>
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

    /// <summary>
    /// Handles changes to the main menu items collection, such as when items are added or removed.
    /// Updates the currently selected item to keep the selection valid (e.g., unsets if removed).
    /// </summary>
    private void MenuItemsCollectionOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Remove:
                foreach (var menuItem in e.OldItems?.OfType<IHamburgerMenuItem>() ?? [])
                {
                    if (SelectedMenuItem == menuItem)
                    {
                        SelectedMenuItem = null;
                    }
                }
                break;
        }
        SelectedMenuItem ??= MenuItems.FirstOrDefault(x => x.Enabled);
    }

    /// <summary>
    /// Tracks whether the pane was open before the last auto-collapse operation.
    /// Used to restore the correct state when the window size increases again.
    /// </summary>
    private bool _wasPaneOpenBeforeAutoCollapse = true;

    /// <summary>
    /// Prevents infinite loops during auto-collapse operations.
    /// Set to true only during automatic state changes.
    /// </summary>
    private bool _isAutoCollapsing;

    /// <summary>
    /// Automatically opens or closes the pane based on window size and threshold.
    /// This helps create responsive behavior—open on large screens, closed on small ones.
    /// </summary>
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