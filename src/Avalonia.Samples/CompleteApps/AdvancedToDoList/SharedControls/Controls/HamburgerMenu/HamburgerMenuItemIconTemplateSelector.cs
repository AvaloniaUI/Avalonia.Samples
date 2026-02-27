using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;

namespace SharedControls.Controls;

/// <summary>
/// A singleton data template selector that automatically renders icons for menu items
/// based on their type (e.g., vector paths or images).
/// This selector handles <see cref="StreamGeometry"/> (path-based icons) and <see cref="IImage"/> ( bitmap images).
/// </summary>
/// <remarks>
/// It's used as a fallback/default for rendering the <see cref="HamburgerMenuItem.Icon"/> property.
/// For example, if your menu item has <c>Icon="M10 10 L20 20"</c> (a path), it becomes a <see cref="PathIcon"/>.  
/// If it has <c>Icon="{DynamicResource SomeImage}"</c>, it becomes an <see cref="Image"/>.
/// </remarks>
public class HamburgerMenuItemIconTemplateSelector : IDataTemplate
{
    /// <summary>
    /// Gets the singleton instance of this template selector.
    /// Ensures only one instance exists and is reused across the app.
    /// </summary>
    public static HamburgerMenuItemIconTemplateSelector Instance { get; } = new();
  
    /// <summary>
    /// Builds the appropriate visual control (e.g., <see cref="PathIcon"/> or <see cref="Image"/>) 
    /// based on the icon's runtime type.
    /// </summary>
    /// <param name="param">The icon value from <see cref="HamburgerMenuItem.Icon"/> (e.g., path geometry or image).</param>
    /// <returns>A new control representing the icon, or <c>null</c> if the type is unsupported.</returns>
    public Control? Build(object? param)
    {
        switch (param)
        {
            case StreamGeometry geometry:
                return new PathIcon()
                {
                    Data = geometry
                };
            case IImage image:
                return new Image()
                {
                    Source = image
                };
            default:
                return null;
        }
    }

    /// <summary>
    /// Checks whether this selector handles the given data object.
    /// Returns <c>true</c> for any object implementing <see cref="Geometry"/> (e.g., <see cref="StreamGeometry"/>) 
    /// or <see cref="IImage"/> (e.g., <see cref="Bitmap"/>).
    /// </summary>
    /// <param name="data">The data object to check.</param>
    /// <returns><c>true</c> if this selector supports the object type; otherwise, <c>false</c>.</returns>
    public bool Match(object? data)
    {
        return data is Geometry or IImage;
    }
}