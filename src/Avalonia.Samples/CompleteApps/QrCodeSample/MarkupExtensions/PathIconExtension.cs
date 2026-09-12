using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace QrCodeSample.MarkupExtensions;

public class PathIconEx : PathIcon
{
    protected override Type StyleKeyOverride => typeof(PathIcon);

    public object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}