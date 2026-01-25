namespace SharedControls.Controls;

public class HamburgerMenuSeparatorItem : HamburgerMenuItem
{
    static HamburgerMenuSeparatorItem()
    {
        EnabledProperty.OverrideDefaultValue<HamburgerMenuSeparatorItem>(false);
        AutoHideProperty.OverrideDefaultValue<HamburgerMenuSeparatorItem>(false);
    }
}