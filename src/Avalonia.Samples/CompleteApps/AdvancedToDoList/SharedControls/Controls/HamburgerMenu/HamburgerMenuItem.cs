using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace SharedControls.Controls;

/// <summary>
/// Represents a single menu item in a HamburgerMenu.
/// It supports icons, labels, enable/disable state, and metadata (Tag).
/// This is the base class for clickable navigation items and header separators.
/// </summary>
/// <example>
/// In XAML, you'd typically use it like:
/// <code language="xaml">
/// <![CDATA[
/// <controls:HamburgerMenuItem Header="Home" Icon="{DynamicResource HomeIcon}" />
/// ]]>
/// </code>
/// </example>
public class HamburgerMenuItem : AvaloniaObject, IHamburgerMenuItem
{
    /// <summary>
    /// Defines the dependency property for whether the menu item is enabled (clickable).
    /// Default is <c>true</c>. Set to <c>false</c> to disable interaction.
    /// </summary>
    public static readonly StyledProperty<bool> EnabledProperty = AvaloniaProperty.Register<HamburgerMenuItem, bool>(
        nameof(Enabled), true);

    /// <summary>
    /// Gets or sets whether the menu item is enabled and can be interacted with.
    /// </summary>
    public bool Enabled
    {
        get => GetValue(EnabledProperty);
        set => SetValue(EnabledProperty, value);
    }
  
    /// <summary>
    /// Defines the dependency property for the optional icon displayed next to the label.
    /// Can be a string (path), icon glyph, drawing, or UIElement.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty = AvaloniaProperty.Register<HamburgerMenuItem, object?>(
        nameof(Icon));
  
    /// <summary>
    /// Gets or sets the icon content (image, path, path geometry, etc.) shown before the label.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Defines the dependency property for a custom template used to render the icon.
    /// This allows full control over how the icon appears (e.g., vector graphics, animated glyphs).
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty = AvaloniaProperty.Register<HamburgerMenuItem, IDataTemplate?>(
        nameof(IconTemplate), HamburgerMenuItemIconTemplateSelector.Instance);

    /// <summary>
    /// Gets or sets the data template used to render the icon.
    /// If <see cref="Icon"/> is set but this is <c>null</c>, a default template may be used.
    /// </summary>
    public IDataTemplate? IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }

    /// <summary>
    /// Defines the dependency property for the main text/label of the menu item.
    /// Marked with <see cref="ContentAttribute"/> so it can be set directly in XAML without a property name.
    /// </summary>
    public static readonly StyledProperty<object?> LabelProperty = AvaloniaProperty.Register<HamburgerMenuItem, object?>(
        nameof(Label));

    /// <summary>
    /// Gets or sets the label/content shown next to the icon.
    /// This is the main visible text (or UI element) of the menu item.
    /// </summary>
    [Content]
    public object? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>
    /// Defines the dependency property for a custom template used to render the label.
    /// Useful when you want to format the text (bold, color, icon + text, etc.).
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> LabelTemplateProperty = AvaloniaProperty.Register<HamburgerMenuItem, IDataTemplate?>(
        nameof(LabelTemplate));

    /// <summary>
    /// Gets or sets the data template used to render the label.
    /// If not set, the raw <see cref="Label"/> value is shown directly.
    /// </summary>
    public IDataTemplate? LabelTemplate
    {
        get => GetValue(LabelTemplateProperty);
        set => SetValue(LabelTemplateProperty, value);
    }

    /// <summary>
    /// Defines the dependency property for arbitrary user data associated with the menu item.
    /// Often used to store routing info, command parameters, or IDs.
    /// </summary>
    public static readonly DirectProperty<HamburgerMenuItem, object?> TagProperty = AvaloniaProperty.RegisterDirect<HamburgerMenuItem, object?>(
        nameof(Tag), o => o.Tag, (o, v) => o.Tag = v);

    /// <summary>
    /// Gets or sets an arbitrary object containing additional data for the menu item.
    /// This is not used by the control itself but can be read by your logic (e.g., view models).
    /// </summary>
    public object? Tag
    {
        get;
        set => SetAndRaise(TagProperty, ref field, value);
    }

    /// <summary>
    /// Defines the dependency property that controls whether the item is automatically hidden when the navigation pane enters compact (collapsed) mode.
    /// Set to <c>true</c> to hide the item when the pane is too narrow to show it comfortably.
    /// </summary>
    public static readonly StyledProperty<bool> AutoHideProperty = AvaloniaProperty.Register<HamburgerMenuItem, bool>(
        nameof(AutoHide), false);

    /// <summary>
    /// Gets or sets whether the menu item should be automatically hidden when the navigation pane is in compact mode (e.g., on small screens).
    /// Default is <c>false</c> → item stays visible even when the pane is narrowed.
    /// Set to <c>true</c> if you want the item to disappear automatically when space is limited.
    /// </summary>
    public bool AutoHide
    {
        get => GetValue(AutoHideProperty);
        set => SetValue(AutoHideProperty, value);
    }
}