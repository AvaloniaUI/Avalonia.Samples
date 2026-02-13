namespace SharedControls.Controls;

/// <summary>
/// A special type of menu item used as a visual separator or label inside a HamburgerMenu.
/// Unlike regular menu items, this one is disabled by default and can't be clicked.
/// It's perfect for grouping related items under headings like "Main Navigation" or "Settings".
/// </summary>
/// <example>
/// You might use it in your menu like this:
/// <code language="xaml">
/// <![CDATA[
/// <controls:HamburgerMenu>
///     <controls:HamburgerMenuHeaderItem Header="Main Actions" />
///     <controls:HamburgerMenuItem Header="New Task" Command="{Binding CreateTask}" />
///     <controls:HamburgerMenuHeaderItem Header="Settings" />
///     <controls:HamburgerMenuItem Header="Options" Command="{Binding OpenOptions}" />
/// </controls:HamburgerMenu>
/// ]]>
/// </code>
/// </example>
public class HamburgerMenuHeaderItem : HamburgerMenuItem
{
    /// <summary>
    /// Initializes static members of the HamburgerMenuHeaderItem class.
    /// Overrides the default value of the Enabled property to false,
    /// ensuring this item is never interactive (no click handler will fire).
    /// </summary>
    static HamburgerMenuHeaderItem()
    {
        EnabledProperty.OverrideDefaultValue<HamburgerMenuHeaderItem>(false);
    }
}