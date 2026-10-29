using System;
using System.Globalization;
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
    private string? _lastInvalidText;
    private bool _hasValidationError;
    private bool _isRollingBack;
    private bool _pendingRestorationCanceled;
    
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

        if (DateTimeHelper.TryAddDays(DateTime.Today, offsetInt, out var selectedDate)
            && BlackoutDates?.Contains(selectedDate) != true)
        {
            // Set the selected date to today + offset
            SetCurrentValue(SelectedDateProperty, selectedDate);
        }
    }
    
    /// <inheritdoc />
    protected override void OnDateValidationError(CalendarDatePickerDateValidationErrorEventArgs e)
    {
        base.OnDateValidationError(e);

        _hasValidationError = true;
        _lastInvalidText = e.Text;

        // Feed the exception to Avalonia's visual validation adorners
        UpdateDataValidation(TextProperty, BindingValueType.DataValidationError, e.Exception);

        // The base parser will rollback the Text property after this method returns.
        // We need to restore the invalid text AFTER the base parsing completes.
        // Use Dispatcher to schedule the restoration after the current processing cycle.
        // Cancel any pending restoration from a previous error, as this new error replaces it.
        CancelPendingRestoration();

        if (_lastInvalidText != null)
        {
            // Reset the cancellation flag so the new restoration callback is not immediately canceled
            _pendingRestorationCanceled = false;
            _isRollingBack = true;
            Dispatcher.UIThread.Post(() =>
            {
                // Only restore if this restoration hasn't been canceled by a valid selection
                if (!_pendingRestorationCanceled && _textBox != null )
                {
                    _textBox.Text = _lastInvalidText;
                }
                _isRollingBack = false;
            });
        }
    }
    
    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TextProperty)
        {
            // If we're currently rolling back from invalid input, don't clear the error state
            // The rollback is the base parser reverting the text, not a user correction
            if (_isRollingBack)
            {
                return;
            }

            // If the user modified the text away from the invalid input, clear the error state
            if (_hasValidationError && change.GetNewValue<string?>() != _lastInvalidText)
            {
                CancelPendingRestoration();
                _hasValidationError = false;
                _lastInvalidText = null;
                UpdateDataValidation(TextProperty, BindingValueType.DataValidationError, null);
            }
        }
        else if (change.Property == SelectedDateProperty)
        {
            // When a valid date is selected (e.g., via a shortcut button), cancel any pending
            // invalid text restoration and clear the validation error state.
            CancelPendingRestoration();
            if (_hasValidationError)
            {
                _hasValidationError = false;
                _lastInvalidText = null;
                UpdateDataValidation(TextProperty, BindingValueType.DataValidationError, null);
            }
        }
    }

    /// <summary>
    /// Cancels any pending invalid text restoration scheduled via the dispatcher.
    /// This is called when a valid date is selected or the user starts typing a new value.
    /// </summary>
    private void CancelPendingRestoration()
    {
        _pendingRestorationCanceled = true;
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