using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace SharedControls.Controls;

/// <summary>
/// A data template selector that automatically chooses the right template for a menu item's content
/// (such as icon or label) based on the object's runtime type.
/// This allows you to define multiple templates (e.g., one for separators, one for regular items)
/// and have the control pick the best match automatically.
/// </summary>
/// <example>
/// The typical usage is to define it as a resource in a ResourceDictionary:
/// <code language="xaml">
/// <![CDATA[
/// <controls:HamburgerMenuItemContentTemplateSelector x:Key="HamburgerMenuItemContentTemplateSelector">
///     <controls:HamburgerMenuItemContentTemplateSelector.DefaultTemplate>
///         <DataTemplate DataType="controls:IHamburgerMenuItem">
///             <!-- Default layout for most items -->
///             <Grid>
///                 <!-- Icon and Label content -->
///             </Grid>
///         </DataTemplate>
///     </controls:HamburgerMenuItemContentTemplateSelector.DefaultTemplate>
///
///     <!-- Specific template for separators -->
///     <DataTemplate x:Key="{x:Type controls:HamburgerMenuSeparatorItem}">
///         <Separator HorizontalAlignment="Stretch" />
///     </DataTemplate>
/// </controls:HamburgerMenuItemContentTemplateSelector>
/// ]]>
/// </code>
/// Then reference it in a ListBox or other items control via <c>{StaticResource HamburgerMenuItemContentTemplateSelector}</c>.
/// </example>
public class HamburgerMenuItemContentTemplateSelector : IDataTemplate
{
    /// <summary>
    /// A collection of type → template mappings used to pick the correct template.
    /// The key is the type (e.g., <see cref="HamburgerMenuSeparatorItem"/>), and the value is the template to use.
    /// Marked with <see cref="ContentAttribute"/> so it can be populated inline in XAML.
    /// </summary>
    [Content]
    public Dictionary<Type, IDataTemplate> DataTemplates { get; } = new();

    /// <summary>
    /// Gets or sets a fallback template used when no exact type match is found in <see cref="DataTemplates"/>.
    /// If <c>null</c> and no match, the item may render nothing.
    /// </summary>
    public IDataTemplate? DefaultTemplate { get; set; }
  
    /// <summary>
    /// Tries to build a visual control for the given data object by finding a matching template.
    /// First checks if a template exists for the object's actual type.
    /// If not found, falls back to the <see cref="DefaultTemplate"/>, if set.
    /// </summary>
    /// <param name="param">The data object to render (e.g., an icon or label).</param>
    /// <returns>A new control representing the data, or <c>null</c> if no template matches.</returns>
    public Control? Build(object? param)
    {
        if(param == null) return null;

        return DataTemplates.TryGetValue(param.GetType(), out var template)
            ? template.Build(param)
            : DefaultTemplate?.Build(param);
    }

    /// <summary>
    /// Checks whether this template selector should handle the given data object.
    /// Currently returns <c>true</c> only for objects implementing <see cref="IHamburgerMenuItem"/>.
    /// </summary>
    /// <param name="data">The data object to check.</param>
    /// <returns><c>true</c> if this selector handles the data; otherwise, <c>false</c>.</returns>
    public bool Match(object? data)
    {
        return data is IHamburgerMenuItem;
    }
}