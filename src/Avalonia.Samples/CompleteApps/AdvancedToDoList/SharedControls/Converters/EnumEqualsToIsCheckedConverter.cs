using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace SharedControls.Converters;

/// <summary>
/// Value converter for binding enum values to IsChecked properties in radio buttons or checkboxes.
/// Enables two-way binding between enum properties and.IsChecked properties.
/// Supports both one-way (enum → bool) and two-way (bool → enum) conversions.
/// </summary>
/// <remarks>
/// Typical use case: Creating radio button groups for enum selection
/// 
/// Example XAML usage:
/// <StackPanel>
///   <RadioButton Content="Option A" 
///               IsChecked="{Binding MyEnumProperty, Converter={x:Static EnumEqualsToIsCheckedConverter.Instance}, ConverterParameter={x:Static local:MyEnum.OptionA}}" />
///   <RadioButton Content="Option B"
///               IsChecked="{Binding MyEnumProperty, Converter={x:Static EnumEqualsToIsCheckedConverter.Instance}, ConverterParameter={x:Static local:MyEnum.OptionB}}" />
///   <RadioButton Content="Option C"
///               IsChecked="{Binding MyEnumProperty, Converter={x:Static EnumEqualsToIsCheckedConverter.Instance}, ConverterParameter={x:Static local:MyEnum.OptionC}}" />
/// </StackPanel>
/// 
/// Note: This converter works best with RadioButton groups where only one option can be selected.
/// For checkboxes, consider different logic as multiple selections may be desired.
/// </remarks>
public class EnumEqualsToIsCheckedConverter : IValueConverter
{
    /// <summary>
    /// Singleton instance of this converter for use in XAML bindings.
    /// Use this static instance instead of creating new ones for better performance.
    /// </summary>
    public static EnumEqualsToIsCheckedConverter Instance { get; } = new EnumEqualsToIsCheckedConverter();

    /// <summary>
    /// Converts an enum value to a boolean indicating if it matches the parameter.
    /// Used to determine if a radio button/checkbox should be checked.
    /// </summary>
    /// <param name="value">
    /// The current enum value from your ViewModel's property.
    /// Example: If bound to MyEnumProperty, this will be the current enum value.
    /// </param>
    /// <param name="targetType">
    /// The type of binding target property (should be bool for IsChecked).
    /// Not used in this converter but required by IValueConverter interface.
    /// </param>
    /// <param name="parameter">
    /// The enum value this control represents.
    /// Set in XAML via ConverterParameter.
    /// Example: ConverterParameter="{x:Static local:MyEnum.OptionA}"
    /// </param>
    /// <param name="culture">
    /// Culture information for localization.
    /// Not used in this converter but required by IValueConverter interface.
    /// </param>
    /// <returns>
    /// true if the bound value equals the parameter (control should be checked),
    /// false otherwise (control should be unchecked).
    /// </returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Check if the enum value matches the parameter value
        return value?.Equals(parameter) ?? false;
    }

    /// <summary>
    /// Converts a boolean back to an enum value for two-way bindings.
    /// Used when user clicks a radio button to update the enum property.
    /// </summary>
    /// <param name="value">
    /// The IsChecked value from the radio button/checkbox (true/false).
    /// When true, user selected this option; when false, they deselected it.
    /// </param>
    /// <param name="targetType">
    /// The type of binding target property (should be enum type).
    /// Not used in this converter but required by IValueConverter interface.
    /// </param>
    /// <param name="parameter">
    /// The enum value this control represents (same as in Convert method).
    /// Returned when IsChecked is true.
    /// </param>
    /// <param name="culture">
    /// Culture information for localization.
    /// Not used in this converter but required by IValueConverter interface.
    /// </param>
    /// <returns>
    /// The parameter enum value when IsChecked is true (selection occurred),
    /// DoNothing when IsChecked is false (deselection occurred - don't change value).
    /// </returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var val = value as bool?;
        
        // Only return the enum value when radio button is checked
        // When unchecked, return DoNothing to avoid changing the enum value
        return val == true ? parameter : BindingOperations.DoNothing;
    }
}