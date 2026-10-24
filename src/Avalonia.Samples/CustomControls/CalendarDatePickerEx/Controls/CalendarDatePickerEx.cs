using System;
using Avalonia.Controls;
using CalendarDatePickerEx.Helper;

namespace CalendarDatePickerEx.Controls;

public class CalendarDatePickerEx : CalendarDatePicker
{
    static CalendarDatePickerEx()
    {
        CalendarDatePicker.TextConverterProperty.OverrideDefaultValue(
            typeof(CalendarDatePickerEx), ShorthandDateConverter.Instance);
    }
    
    public void SelectDayOffset(object offset)
    {
        var offsetInt = Convert.ToInt32(offset);
        
        // Close the popup first. Otherwise, the parsing will kick in and override the selected date.
        SetCurrentValue(IsDropDownOpenProperty, false);
        
        // Set the selected date to today + offset
        SetCurrentValue(SelectedDateProperty, DateTime.Today.AddDays(offsetInt));
    }
}