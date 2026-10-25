using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using CalendarDatePickerEx.Helper;

namespace CalendarDatePickerEx.Controls;

/// <summary>
/// An extended CalendarDatePicker control that supports shorthand date input, validation error handling
/// and selecting a date based on an offset from today.
/// </summary>
public class CalendarDatePickerEx : CalendarDatePicker
{
    private TextBox? _textBox;
    private string? _lastInvalidText;
    private bool _hasValidationError;
    private bool _isValidatingError;
    
    static CalendarDatePickerEx()
    {
        // Override the default TextConverter to use the ShorthandDateConverter for parsing shorthand date inputs.
        TextConverterProperty.OverrideDefaultValue(
            typeof(CalendarDatePickerEx), ShorthandDateConverter.Instance);
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _textBox = e.NameScope.Find<TextBox>("PART_TextBox");
    }

    /// <summary>
    /// Selects a date based on an offset from today. The offset can be provided as an integer, string,
    /// or any object that can be converted to an integer.
    /// </summary>
    /// <param name="offset">the offset in total days.</param>
    public void SelectDayOffset(object? offset)
    {
        if (!TryParseOffset(offset, out var offsetInt))
        {
            throw new ArgumentException("Offset must be an integer or a string parseable to an integer.",
                nameof(offset));
        }

        // Close the popup first. Otherwise, the parsing will kick in and override the selected date.
        SetCurrentValue(IsDropDownOpenProperty, false);

        if (DateTimeHelper.TryAddDays(DateTime.Today, offsetInt, out var selectedDate))
        {
            // Set the selected date to today + offset
            SetCurrentValue(SelectedDateProperty, selectedDate);
        }
    }

    /// <summary>
    /// A helper method to parse the offset value into an integer. It supports various input types,
    /// including int, string, and any object that can be converted to an integer.
    /// </summary>
    /// <param name="offset">the offset parameter to parse.</param>
    /// <param name="result">the result of the parsing operation.</param>
    /// <returns>true if the offset was successfully parsed into an integer, false otherwise.</returns>
    private static bool TryParseOffset(object? offset, out int result)
    {
        result = 0;
        if (offset is null)
            return false;

        switch (offset)
        {
            case int intVal:
                result = intVal;
                return true;
            case string str when int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed):
                result = parsed;
                return true;
            default:
                try
                {
                    result = Convert.ToInt32(offset, CultureInfo.InvariantCulture);
                    return true;
                }
                catch
                {
                    return false;
                }
        }
    }

    /// <summary>
    /// Overrides the base DateValidationError handler to feed validation errors to the
    /// DataValidationErrors wrapper in the control template, so the user sees the error message.
    /// The base CalendarDatePicker only fires an event but never calls UpdateDataValidation.
    /// </summary>
    protected override void OnDateValidationError(CalendarDatePickerDateValidationErrorEventArgs e)
    {
        base.OnDateValidationError(e);

        _isValidatingError = true;
        _hasValidationError = true;
        _lastInvalidText = e.Text;

        // Feed the exception to Avalonia's visual validation adorners
        UpdateDataValidation(TextProperty, BindingValueType.DataValidationError, e.Exception);

        // Restore the invalid text into the TextBox so it does not get reverted to SelectedDate
        if (_textBox != null && _lastInvalidText != null)
        {
            _textBox.Text = _lastInvalidText;
        }
        
        _isValidatingError = false;
    }

    /// <summary>
    /// Clears validation errors when the text property changes successfully.
    /// </summary>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        // Clear validation errors when Text changes (user is typing a new value)
        if (change.Property == TextProperty && !_isValidatingError)
        {
            // If the user modified the text away from the invalid input, clear the error state
            if (_hasValidationError && change.GetNewValue<string?>() != _lastInvalidText)
            {
                _hasValidationError = false;
                _lastInvalidText = null;
                UpdateDataValidation(TextProperty, BindingValueType.DataValidationError, null);
            }
        }
    }
}