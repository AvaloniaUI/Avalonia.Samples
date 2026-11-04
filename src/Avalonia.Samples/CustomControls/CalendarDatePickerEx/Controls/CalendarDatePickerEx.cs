using System;
using System.Globalization;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Threading;
using CalendarDatePickerEx.Helper;

namespace CalendarDatePickerEx.Controls;

/// <summary>
/// An extended CalendarDatePicker control that supports shorthand date input, validation error handling
/// and selecting a date based on an offset from today.
/// </summary>
public class CalendarDatePickerEx : CalendarDatePicker
{
    private TextBox? _textBox;
    private CancellationTokenSource? _restoreTextCts;
    
    static CalendarDatePickerEx()
    {
        // Override the default TextConverter to use the ShorthandDateConverter for parsing shorthand date inputs.
        TextConverterProperty.OverrideDefaultValue(
            typeof(CalendarDatePickerEx), ShorthandDateConverter.Instance);
    }

    // Uncomment the following line if you want your control to reuse the style of the base CalendarDatePicker control.
    // Otherwise, you will need to ship your own ControlTheme. 
    // protected override Type StyleKeyOverride => typeof(CalendarDatePicker);
    
    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _restoreTextCts?.Cancel();
        _restoreTextCts = null;
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

        if (DateTimeHelper.TryAddDays(DateTime.Today, offsetInt, out var selectedDate)
            && BlackoutDates?.Contains(selectedDate) != true)
        {
            SetCurrentValue(SelectedDateProperty, selectedDate);
            ClearValidationError();

            var formattedText = FormatDate(selectedDate);
            SetCurrentValue(TextProperty, formattedText);
            if (_textBox != null)
            {
                _textBox.Text = formattedText;
            }
        }
    }
    
    /// <inheritdoc />
    protected override void OnDateValidationError(CalendarDatePickerDateValidationErrorEventArgs e)
    {
        base.OnDateValidationError(e);

        // Reset the selected date to null on invalid input while retaining the validation error
        SetCurrentValue(SelectedDateProperty, null);
        UpdateDataValidation(TextProperty, BindingValueType.DataValidationError, e.Exception);

        // Keep the invalid text in the TextBox, but guard against the text being cleared
        // or changed before this callback runs by cancelling any pending restoration first.
        var invalidText = e.Text;
        _restoreTextCts?.Cancel();
        var cts = new CancellationTokenSource();
        _restoreTextCts = cts;
        Dispatcher.UIThread.Post(() =>
        {
            if (cts.IsCancellationRequested) return;
            if (_textBox != null && SelectedDate == null)
            {
                _textBox.Text = invalidText;
            }
            if (_restoreTextCts == cts)
                _restoreTextCts = null;
        });
    }
    
    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedDateProperty && change.GetNewValue<DateTime?>() != null)
        {
            ClearValidationError();
        }
        else if (change.Property == TextProperty)
        {
            // Cancel any pending text restoration so stale text isn't written after a new edit.
            _restoreTextCts?.Cancel();
            _restoreTextCts = null;

            if (string.IsNullOrEmpty(change.GetNewValue<string?>()))
            {
                ClearValidationError();
            }
        }
    }

    private void ClearValidationError()
    {
        UpdateDataValidation(TextProperty, BindingValueType.DataValidationError, null);
    }
    
    /// <summary>
    /// Formats a DateTime into a string based on the configured TextConverter or SelectedDateFormat.
    /// </summary>
    /// <param name="date">the date to format.</param>
    /// <returns>the formatted string representation of the date.</returns>
    private string? FormatDate(DateTime date)
    {
        if (TextConverter != null)
        {
            return TextConverter.Convert(date, typeof(string), null, CultureInfo.CurrentCulture) as string;
        }

        return SelectedDateFormat switch
        {
            CalendarDatePickerFormat.Short => date.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, CultureInfo.CurrentCulture),
            CalendarDatePickerFormat.Long => date.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern, CultureInfo.CurrentCulture),
            CalendarDatePickerFormat.Custom => string.IsNullOrEmpty(CustomDateFormatString)
                ? date.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, CultureInfo.CurrentCulture)
                : date.ToString(CustomDateFormatString, CultureInfo.CurrentCulture),
            _ => date.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, CultureInfo.CurrentCulture)
        };
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
}