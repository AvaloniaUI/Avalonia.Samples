using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace SharedControls.Controls;

public class HamburgerMenuItemIconTemplateSelector : IDataTemplate
{
    public static HamburgerMenuItemIconTemplateSelector Instance { get; } = new();
    
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

    public bool Match(object? data)
    {
        return data is Geometry or IImage;
    }
}