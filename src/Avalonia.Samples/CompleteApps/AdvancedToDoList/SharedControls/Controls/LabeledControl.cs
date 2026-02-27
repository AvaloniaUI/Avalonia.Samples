using Avalonia;
using Avalonia.Controls.Primitives;
using SharedControls.Converters;

namespace SharedControls.Controls;

/// <summary>
/// A container control that displays a header alongside content — ideal for labeled form sections.
/// Built on top of <see cref="HeaderedContentControl"/>, it supports flexible header positioning
/// to adapt to different screen sizes (e.g., left-aligned on desktop, stacked on mobile).
/// </summary>
/// <remarks>
/// Use this control to wrap inputs, displays, or any grouped content where a label improves clarity.
/// Common scenarios:
/// - Form fields (e.g., "Name", "Email", "Due Date")
/// - Read-only info blocks (e.g., "Created on", "Status")
/// - Custom input groups (e.g., slider and value label)
/// 
/// Header positioning is responsive by design:
/// - <see cref="HeaderPosition.Left"/>: Traditional desktop form layout — labels beside content
/// - <see cref="HeaderPosition.Top"/>: Compact layout — labels above content, better for narrow screens
/// 
/// For automatic responsiveness, bind <see cref="HeaderPosition"/> using 
/// <see cref="BoundsToHeaderPositionConverter"/>.
/// </remarks>
/// <example>
/// Basic usage in XAML:
/// <code language="xaml">
/// <![CDATA[
/// <controls:LabeledControl Header="Title">
///     <TextBox Text="{Binding Title}" Watermark="Enter a title..." />
/// </controls:LabeledControl>
/// ]]>
/// </code>
/// </example>
public class LabeledControl : HeaderedContentControl
{
    /// <summary>
    /// Defines the dependency property for <see cref="HeaderPosition"/>.
    /// Allows customizing where the header appears relative to the content.
    /// </summary>
    public static readonly StyledProperty<HeaderPosition> HeaderPositionProperty =
        AvaloniaProperty.Register<LabeledControl, HeaderPosition>(nameof(HeaderPosition));
  
    /// <summary>
    /// Gets or sets how the header is positioned in relation to the content.
    /// This controls the layout direction and is ideal for responsive UIs.
    /// </summary>
    /// <value>
    /// <see cref="HeaderPosition.Left"/> — Header appears to the left (e.g., desktop layouts).
    /// <see cref="HeaderPosition.Top"/> — Header appears above (e.g., mobile or narrow layouts).
    /// </value>
    public HeaderPosition HeaderPosition
    {
        get { return GetValue(HeaderPositionProperty); }
        set { SetValue(HeaderPositionProperty, value); }
    }
}

/// <summary>
/// Specifies where the header should appear relative to the content in <see cref="LabeledControl"/>.
/// </summary>
/// <remarks>
/// Choose <see cref="Left"/> for desktop-style forms and <see cref="Top"/> for mobile or stacked layouts.
/// For fully responsive behavior, combine with screen size converters (e.g., <see cref="BoundsToHeaderPositionConverter"/>).
/// </remarks>
public enum HeaderPosition
{
    /// <summary>
    /// Positions the header to the left of the content.
    /// Best for wide screens and traditional form layouts (e.g., desktop apps).
    /// </summary>
    Left,

    /// <summary>
    /// Positions the header above the content.
    /// Best for narrow screens or vertical stacking (e.g., mobile apps).
    /// </summary>
    Top
}
