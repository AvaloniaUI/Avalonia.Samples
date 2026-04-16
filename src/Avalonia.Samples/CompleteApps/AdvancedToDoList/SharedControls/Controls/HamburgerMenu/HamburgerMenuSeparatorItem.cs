namespace SharedControls.Controls;

/// <summary>
/// A special type of menu item that acts as a visual separator (like a horizontal line) 
/// between groups of navigation items in a HamburgerMenu.
/// Unlike regular menu items, this one is disabled (non-clickable) and hidden automatically 
/// when the navigation pane enters compact mode (to save space on small screens).
/// </summary>
/// <example>
/// Use it in your XAML like this:
/// <code language="xaml">
/// <![CDATA[
/// <controls:HamburgerMenu>
///     <controls:HamburgerMenuItem Header="Home" />
///     <controls:HamburgerMenuSeparatorItem />
///     <controls:HamburgerMenuItem Header="Settings" />
/// </controls:HamburgerMenu>
/// ]]>
/// </code>
/// </example>
public class HamburgerMenuSeparatorItem : HamburgerMenuItem
{
    /// <summary>
    /// Initializes static members of the HamburgerMenuSeparatorItem class.
    /// Overrides defaults to ensure:
    /// - <see cref="Enabled"/> is <c>false</c> (so it cannot be clicked)
    /// - <see cref="AutoHide"/> is <c>true</c> (so it hides automatically when the pane is narrow)
    /// </summary>
    static HamburgerMenuSeparatorItem()
    {
        EnabledProperty.OverrideDefaultValue<HamburgerMenuSeparatorItem>(false);
        AutoHideProperty.OverrideDefaultValue<HamburgerMenuSeparatorItem>(true);
    }
}