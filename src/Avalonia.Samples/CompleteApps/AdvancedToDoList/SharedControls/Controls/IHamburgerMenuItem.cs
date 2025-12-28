using Avalonia.Controls.Templates;
using Avalonia.Media;

namespace SharedControls.Controls;

public interface IHamburgerMenuItem
{
    bool Enabled { get; set; }
    
    object? Icon { get; set; }
    IDataTemplate? IconTemplate { get; set; }
    
    object? Label { get; set; }
    IDataTemplate? LabelTemplate { get; set; }
    
    object? Tag { get; set; }
    
    bool AutoHide { get; set; }
}