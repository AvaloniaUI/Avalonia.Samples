namespace SharedControls.Controls;

public class HamburgerMenuHeaderItem : HamburgerMenuItem
{
    static HamburgerMenuHeaderItem()
    {
        EnabledProperty.OverrideDefaultValue<HamburgerMenuHeaderItem>(false);
    }
}