using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace SharedControls.Controls;

public class HamburgerMenuItemContentTemplateSelector : IDataTemplate
{
    [Content]
    public Dictionary<Type, IDataTemplate> DataTemplates { get; } = new();

    public IDataTemplate? DefaultTemplate { get; set; }
    
    public Control? Build(object? param)
    {
        if(param == null) return null;

        return DataTemplates.TryGetValue(param.GetType(), out var template)
            ? template.Build(param)
            : DefaultTemplate?.Build(param);
    }

    public bool Match(object? data)
    {
        return data is IHamburgerMenuItem;
    }
}