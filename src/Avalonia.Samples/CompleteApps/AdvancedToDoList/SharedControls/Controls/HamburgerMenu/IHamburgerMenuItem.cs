using Avalonia.Controls.Templates;

namespace SharedControls.Controls;

/// <summary>
/// Defines the contract for items that can appear in a <see cref="HamburgerMenu"/>,
/// such as navigation items, headers, or separators.
/// All properties are optional, allowing flexible customization of how each item appears and behaves.
/// </summary>
/// <remarks>
/// Implementations typically inherit from <see cref="HamburgerMenuItem"/> or 
/// <see cref="HamburgerMenuHeaderItem"/>/<see cref="HamburgerMenuSeparatorItem"/>.
/// </remarks>

public interface IHamburgerMenuItem
{
    /// <summary>
    /// Gets or sets whether the menu item is enabled and can be interacted with (clicked).
    /// Default is <c>true</c> for regular items; <c>false</c> for headers/separators.
    /// </summary>
    bool Enabled { get; set; }
  
    /// <summary>
    /// Gets or sets the icon displayed next to the label (e.g., text, emoji, path geometry, or image).
    /// The actual rendering is handled by a template selector (e.g., <see cref="HamburgerMenuItemIconTemplateSelector"/>).
    /// </summary>
    object? Icon { get; set; }

    /// <summary>
    /// Gets or sets a custom data template used to render the <see cref="Icon"/> property.
    /// If not set, a default template (based on the icon's type) is used.
    /// </summary>
    IDataTemplate? IconTemplate { get; set; }
  
    /// <summary>
    /// Gets or sets the main text or UI element shown as the menu item's label.
    /// </summary>
    object? Label { get; set; }

    /// <summary>
    /// Gets or sets a custom data template used to render the <see cref="Label"/> property.
    /// Useful for applying formatting, icons, or complex layouts to the label.
    /// </summary>
    IDataTemplate? LabelTemplate { get; set; }
  
    /// <summary>
    /// Gets or sets an arbitrary object containing additional data for the menu item.
    /// This is not used by the control itself but can store routing info, commands, or view models.
    /// </summary>
    object? Tag { get; set; }
  
    /// <summary>
    /// Gets or sets whether the menu item should be automatically hidden when the navigation pane
    /// enters compact mode (e.g., on small screens).
    /// Default is <c>false</c> for regular items; <c>true</c> for separators.
    /// </summary>
    bool AutoHide { get; set; }
}