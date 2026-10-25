using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Avalonia.Data.Converters;

namespace CalendarDatePickerEx.Helper;

/// <summary>
/// A value converter that parses shorthand date formats into DateTime objects.
/// It supports numeric offsets, week offsets, and ISO week formats.
/// </summary>
public partial class ShorthandDateConverter : IValueConverter
{
    /// <summary>
    /// Gets the singleton instance of the <see cref="ShorthandDateConverter"/> class.
    /// </summary>
    public static ShorthandDateConverter Instance { get; } = new();

    // Regex patterns for shorthand date formats
    /// <summary>
    /// Matches a numeric offset (e.g., "+3", "-2", "0") representing days relative to today.
    /// </summary>
    [GeneratedRegex(@"^[+-]?(\d+)$")]
    private static partial Regex NumericOffsetRegex();

    /// <summary>
    /// Matches a week offset (e.g., "+2w", "-1w.3") representing weeks relative to today,
    /// with an optional day offset (1 = Monday to 7 = Sunday) after a dot.
    /// </summary>
    [GeneratedRegex(@"^([+-]?\d+)[wW](?:\.(\d+))?$")]
    private static partial Regex WeekOffsetRegex();

    /// <summary>
    /// Matches an ISO week format (e.g., "W12", "W12.3", "W12/2024") representing a specific week of the year,
    /// with an optional day offset (1 = Monday to 7 = Sunday) after a dot, and an optional year after a slash.
    /// </summary>
    [GeneratedRegex(@"^[wW](\d+)(?:\.(\d+))?(?:/(\d+))?$")]
    private static partial Regex IsoWeekRegex();

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateTime dateTime)
            return null;

        var dtfi = culture.DateTimeFormat;
        return dateTime.ToString(dtfi.ShortDatePattern, dtfi);
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // ensure the value is a non-empty string
        if (value is not string text || string.IsNullOrWhiteSpace(text))
            return null;

        // trim whitespace from the input string
        text = text.Trim();

        // Try shorthand formats first: numeric offset, week offset, ISO week
        if (TryParseNumericOffset(text, out var numericDate))
            return numericDate;

        if (TryParseWeekOffset(text, out var weekDate))
            return weekDate;

        if (TryParseIsoWeek(text, out var isoDate))
            return isoDate;

        // Fallback: try general date parsing using the culture passed by the framework
        var dtfi = culture.DateTimeFormat;
        if (DateTime.TryParse(text, dtfi, DateTimeStyles.None, out var parsedDate))
        {
            return parsedDate;
        }

        throw new FormatException($"Cannot parse the given text to a date: {text}");
    }

    /// <summary>
    /// Tries to parse a numeric offset string (e.g., "+3", "-2", "0") into a DateTime relative to today.
    /// </summary>
    /// <param name="text">the text to parse.</param>
    /// <param name="result">the parsed date.</param>
    /// <returns>true if the text was successfully parsed, otherwise false.</returns>
    /// <exception cref="FormatException">if the format was invalid.</exception>
    private static bool TryParseNumericOffset(string text, out DateTime result)
    {
        result = default;

        // check if the text matches the numeric offset regex.
        if (!NumericOffsetRegex().IsMatch(text))
            return false;

        // try to parse the text as an integer. If it fails, throw a FormatException.
        if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var offset))
            throw new FormatException($"Day offset '{text}' is out of range.");

        // try to add the offset to today's date. If it fails, throw a FormatException.
        if (!DateTimeHelper.TryAddDays(DateTime.Today, offset, out result))
            throw new FormatException($"Day offset {offset} is out of range.");

        // if everything succeeded, return true.
        return true;
    }

    /// <summary>
    /// Tries to parse a week offset string (e.g., "+2w", "-1w.3") into a DateTime relative to today.
    /// </summary>
    /// <param name="text">the text to parse.</param>
    /// <param name="result">the parsed date.</param>
    /// <returns>true if the text was successfully parsed, otherwise false.</returns>
    /// <exception cref="FormatException">if the format was invalid.</exception>
    private static bool TryParseWeekOffset(string text, out DateTime result)
    {
        result = default;

        // check if the text matches the week offset regex.
        var match = WeekOffsetRegex().Match(text);
        if (!match.Success)
            return false;

        // try to parse the week offset as an integer. If it fails, throw a FormatException.
        if (!int.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture,
                out var weekOffset))
            throw new FormatException($"Week offset '{match.Groups[1].Value}' is out of range.");

        // try to parse the day offset (if any) as an integer. If it fails, throw a FormatException.
        // The day offset is optional. If it is not specified, it defaults to 0, which will be ignored.
        if (!TryParseDayOffset(match.Groups[2], out var dayOffset))
            throw new FormatException($"Invalid day offset in shorthand date: {text}. Use 1-7 for Monday-Sunday.");

        // calculate the total number of days to add to today's date based on the week offset.
        var totalDays = (long)weekOffset * 7;
        if (!DateTimeHelper.TryAddDays(DateTime.Today, totalDays, out var targetDate))
            throw new FormatException($"Week offset {weekOffset} produces a date out of range.");

        // if the day offset is 0 or negative, return the target date.
        if (dayOffset <= 0)
        {
            result = targetDate;
            return true;
        }

        // calculate the number of days since Monday for the target date and adjust it to the
        // specified day of week.
        var daysSinceMonday = ((int)targetDate.DayOfWeek + 6) % 7;
        var dayAdjustment = dayOffset - 1 - daysSinceMonday;

        if (!DateTimeHelper.TryAddDays(targetDate, dayAdjustment, out result))
        {
            throw new FormatException($"Week offset '{text}' produces a date out of range.");
        }

        // if everything succeeded, return true.
        return true;
    }

    /// <summary>
    /// Tries to parse an ISO week string (e.g., "W12", "W12.3", "W12/2024") into a DateTime.
    /// </summary>
    /// <param name="text">the text to parse.</param>
    /// <param name="result">the parsed date.</param>
    /// <returns>true if the text was successfully parsed; otherwise, false.</returns>
    /// <exception cref="FormatException">if the text is not a valid ISO week string.</exception>
    private static bool TryParseIsoWeek(string text, out DateTime result)
    {
        result = default;

        // check if the text matches the ISO week regex.
        var match = IsoWeekRegex().Match(text);
        if (!match.Success)
            return false;

        // try to parse the week number as an integer. If it fails, throw a FormatException.
        if (!int.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture,
                out var weekNumber))
            throw new FormatException($"ISO week '{match.Groups[1].Value}' is out of range.");

        // try to parse the day offset (if any) as an integer. If it fails, throw a FormatException.
        if (!TryParseDayOffset(match.Groups[2], out var dayOffset))
            throw new FormatException($"Invalid day offset in shorthand date: {text}. Use 1-7 for Monday-Sunday.");

        // determine the year to use. If a year is specified, use it; otherwise, use the current year.
        var year = DateTime.Today.Year;
        if (match.Groups[3].Success)
        {
            if (!int.TryParse(match.Groups[3].Value, NumberStyles.Integer, CultureInfo.InvariantCulture,
                    out var parsedYear) || parsedYear < 0)
                throw new FormatException($"Invalid year in shorthand date: {text}.");

            year = parsedYear >= 100 ? parsedYear : 2000 + parsedYear;
        }

        // calculate the weekday based on the day offset. If no day offset is specified, default to Monday.
        var dayOfWeek = dayOffset > 0
            ? (DayOfWeek)(dayOffset % 7)
            : DayOfWeek.Monday;

        // get the date of the specified ISO week and day of week. If it fails, throw a FormatException.
        var targetDate = DateTimeHelper.GetDateOfIsoWeek(year, weekNumber, dayOfWeek);
        if (targetDate is null)
            throw new FormatException($"ISO week {weekNumber} does not exist in {year}.");

        // if everything succeeded, return true.
        result = targetDate.Value;
        return true;
    }

    /// <summary>
    /// Tries to parse a day offset from a regex group. The day offset is expected to be an integer between 1 and 7,
    /// where 1 represents Monday and 7 represents Sunday. If the group is empty, it defaults to 0 (no offset).
    /// Returns true if the parsing is successful or if the group is empty; otherwise, returns false.
    /// </summary>
    /// <param name="group">the regex group containing the day offset.</param>
    /// <param name="dayOffset">the day offset parsed from the regex group.</param>
    /// <returns>true if the parsing is successful or if the group is empty; otherwise, returns false.</returns>
    private static bool TryParseDayOffset(Group group, out int dayOffset)
    {
        dayOffset = 0;
        if (!group.Success)
            return true; // No day offset specified, default to Monday
        if (!int.TryParse(group.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedDay))
            return false; // Non-numeric after dot is an error
        if (parsedDay is < 1 or > 7)
            return false; // Invalid day offset
        dayOffset = parsedDay;
        return true;
    }
}