using System;
using System.Globalization;

namespace CalendarDatePickerEx.Helper;

public static class DateTimeHelper
{
    /// <summary>
    /// Attempts to add a specified number of days to a base date,
    /// ensuring that the resulting date is within the valid range of DateTime.
    /// </summary>
    /// <param name="baseDate">the base date to which the days will be added.</param>
    /// <param name="days">the number of days to add</param>
    /// <param name="result">the resulting date after adding the specified number of days.</param>
    /// <returns>true if the operation was successful, false otherwise.</returns>
    public static bool TryAddDays(DateTime baseDate, long days, out DateTime result)
    {
        var minDaysAllowed = (DateTime.MinValue.Date - baseDate.Date).Days;
        var maxDaysAllowed = (DateTime.MaxValue.Date - baseDate.Date).Days;

        // Check if the number of days to add is within the valid range
        if (days < minDaysAllowed || days > maxDaysAllowed)
        {
            result = default;
            return false;
        }

        result = baseDate.AddDays(days);
        return true;
    }

    /// <summary>
    /// Returns the date of a specified day within an ISO week and year.
    /// Defaults to Monday if not specified.
    /// </summary>
    /// <param name="year">the year for which the date of the specified ISO week is to be returned.</param>
    /// <param name="weekNumber">the ISO week number for which the date is to be returned.</param>
    /// <param name="dayOfWeek">the day of the week (defaults to Monday).</param>
    /// <returns>The calculated date, or null if the input values are invalid.</returns>
    public static DateTime? GetDateOfIsoWeek(int year, int weekNumber, DayOfWeek dayOfWeek = DayOfWeek.Monday)
    {
        // Validate year is within DateTime range
        if (year is < 1 or > 9999)
            return null;

        // Validate week number is within valid ISO range (1–53)
        if (weekNumber < 1 || weekNumber > 53)
            return null;

        // Check that the requested year actually has the requested week
        if (weekNumber > ISOWeek.GetWeeksInYear(year))
            return null;

        try
        {
            var result = ISOWeek.ToDateTime(year, weekNumber, dayOfWeek);

            // Verify the resulting date's ISO year matches the requested year.
            // ISOWeek.ToDateTime may return a date whose ISO year differs from
            // the requested year for edge cases near year boundaries.
            if (ISOWeek.GetYear(result) != year)
                return null;

            return result;
        }
        catch
        {
            // Year is outside DateTime range or other conversion error
            return null;
        }
    }
}