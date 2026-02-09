using Avalonia;
using Avalonia.Controls.Primitives;

namespace SharedControls.Controls;

/// <summary>
/// A control that displays content with an optional header/label.
/// Extends HeaderedContentControl to provide flexible header positioning.
/// Useful for forms, input groups, or any labeled content area.
/// </summary>
/// <remarks>
/// Features:
/// - Automatic header with title/description
/// - Flexible positioning: Left (desktop) or Top (mobile/responsive)
/// - HeaderedContentControl provides base functionality
/// 
/// When to use LabeledControl:
/// - Form groups with titles
/// - Input sections with descriptions
/// - Any content needing labeled container
/// 
/// Header Positioning:
/// - Left: Good for wide screens, desktop layouts
/// - Top: Better for narrow screens, mobile layouts
/// - Can be controlled via BoundsToHeaderPositionConverter for responsiveness
/// </remarks>
/// <example>
/// XAML usage:
/// <code>
/// &lt;controls:LabeledControl Header="Personal Information"
///                           HeaderPosition="Left"&gt;
///     &lt;StackPanel Spacing="10"&gt;
///         &lt;TextBox Watermark="Name" /&gt;
///         &lt;TextBox Watermark="Email" /&gt;
///     &lt;/StackPanel&gt;
/// &lt;/controls:LabeledControl&gt;
/// </code>
/// </example>
public class LabeledControl : HeaderedContentControl
{
    /// <summary>
    /// Defines the HeaderPosition styled property for this control.
    /// Allows header to be positioned on the left or top of content.
    /// Can be set in XAML or via binding for dynamic positioning.
    /// </summary>
    public static readonly StyledProperty<HeaderPosition> HeaderPositionProperty =
        AvaloniaProperty.Register<LabeledControl, HeaderPosition>(nameof(HeaderPosition));
    
    /// <summary>
    /// Gets or sets the position of the header relative to content.
    /// Determines whether header appears on the left side (for wide layouts)
    /// or on the top (for narrow/mobile layouts).
    /// </summary>
    /// <value>
    /// Left: Header appears to the left of content (good for desktop/wide screens)
    /// Top: Header appears above content (good for mobile/narrow screens)
    /// </value>
    public HeaderPosition HeaderPosition
    {
        get { return GetValue(HeaderPositionProperty); }
        set { SetValue(HeaderPositionProperty, value); }
    }
}

/// <summary>
/// Enumeration of possible header positions in LabeledControl.
/// Determines layout arrangement of header and content areas.
/// </summary>
/// <remarks>
/// Use these values to control where label appears:
/// - Left: Traditional desktop form layout with labels beside fields
/// - Top: Compact mobile layout with labels above fields
/// - Can be combined with BoundsToHeaderPositionConverter for automatic responsiveness
/// </remarks>
public enum HeaderPosition
{
    /// <summary>
    /// Position header to the left of the content.
    /// Best for desktop applications with wide screens.
    /// Creates a classic form layout with labels adjacent to input fields.
    /// </summary>
    Left,
    
    /// <summary>
    /// Position header above the content.
    /// Best for mobile applications or narrow windows.
    /// Creates a compact layout with labels stacked above content.
    /// </summary>
    Top
}
