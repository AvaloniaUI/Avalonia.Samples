using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Metadata;

namespace SharedControls.Controls;

public class HamburgerMenuItem : AvaloniaObject, IHamburgerMenuItem
{
    public static readonly StyledProperty<bool> EnabledProperty = AvaloniaProperty.Register<HamburgerMenuItem, bool>(
        nameof(Enabled), true);

    public bool Enabled
    {
        get => GetValue(EnabledProperty);
        set => SetValue(EnabledProperty, value);
    }
    
    public static readonly StyledProperty<object?> IconProperty = AvaloniaProperty.Register<HamburgerMenuItem, object?>(
        nameof(Icon));
    
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty = AvaloniaProperty.Register<HamburgerMenuItem, IDataTemplate?>(
        nameof(IconTemplate), HamburgerMenuItemIconTemplateSelector.Instance);

    public IDataTemplate? IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }

    public static readonly StyledProperty<object?> LabelProperty = AvaloniaProperty.Register<HamburgerMenuItem, object?>(
        nameof(Label));

    [Content]
    public object? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate?> LabelTemplateProperty = AvaloniaProperty.Register<HamburgerMenuItem, IDataTemplate?>(
        nameof(LabelTemplate));

    public IDataTemplate? LabelTemplate
    {
        get => GetValue(LabelTemplateProperty);
        set => SetValue(LabelTemplateProperty, value);
    }

    public static readonly DirectProperty<HamburgerMenuItem, object?> TagProperty = AvaloniaProperty.RegisterDirect<HamburgerMenuItem, object?>(
        nameof(Tag), o => o.Tag, (o, v) => o.Tag = v);

    public object? Tag
    {
        get;
        set => SetAndRaise(TagProperty, ref field, value);
    }

    public static readonly StyledProperty<bool> AutoHideProperty = AvaloniaProperty.Register<HamburgerMenuItem, bool>(
        nameof(AutoHide), true);

    public bool AutoHide
    {
        get => GetValue(AutoHideProperty);
        set => SetValue(AutoHideProperty, value);
    }
}