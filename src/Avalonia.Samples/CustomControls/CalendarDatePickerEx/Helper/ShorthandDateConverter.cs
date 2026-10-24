using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace CalendarDatePickerEx.Helper;

public class ShorthandDateConverter : IValueConverter
{
    public static ShorthandDateConverter Instance { get; } = new ShorthandDateConverter();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateTime dateTime)
            return null;

        DateTimeFormatInfo dtfi = culture.DateTimeFormat;
        return dateTime.ToString(dtfi.ShortDatePattern, dtfi);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string text)
            return null;

        text = text.Trim();
        if (text.Length == 0)
            return null;

        // Try shorthand formats first, then fall back to general date parsing.

        // Try numeric offset: 0 = today, 1 = tomorrow, -1 = yesterday, etc.
        if (int.TryParse(text, out int offset))
        {
            return DateTime.Today.AddDays(offset);
        }

        // Try week offset: 3w = in 3 weeks, -3w = 3 weeks ago, 3w.2 = in 3 weeks on Tuesday
        int dayOffset = 0;
        string weekText = text;
        int dotIdx = text.IndexOf('.');
        if (dotIdx >= 0)
        {
            weekText = text.Substring(0, dotIdx);
            string dayPart = text.Substring(dotIdx + 1);
            if (int.TryParse(dayPart, out int parsedDay))
            {
                // Validate day offset is within range 1-7 (Monday-Sunday)
                if (parsedDay < 1 || parsedDay > 7)
                    return null;
                dayOffset = parsedDay;
            }
        }

        if (weekText.Length >= 2 && weekText[weekText.Length - 1] == 'w' && int.TryParse(weekText.Substring(0, weekText.Length - 1), out int weekOffset))
        {
            // Find Monday of the current week first, then add weekOffset weeks, then add dayOffset to reach the target day
            // dayOffset is 1-based: 1=Monday, 2=Tuesday, etc. (0 means no offset specified, default to Monday)
            int daysSinceMonday = (int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Monday;
            DateTime mondayThisWeek = DateTime.Today.AddDays(-daysSinceMonday);
            DateTime monday = mondayThisWeek.AddDays(weekOffset * 7);
            return monday.AddDays(dayOffset > 0 ? dayOffset - 1 : 0);
        }

        // Try ISO week formats:
        // w43 = Monday of ISO week 43, current year
        // w43.2 = Tuesday of ISO week 43, current year
        // w43/25 = Monday of ISO week 43 in 2025
        // w43.2/45 = Tuesday of ISO week 43 in 2045
        if (text.Length >= 2 && text[0] == 'w')
        {
            int year = DateTime.Now.Year;
            int isoDayOffset = 0; // 0 = Monday

            // Parse the rest after 'w': weekNumber[.dayOffset][/YY]
            string rest = text.Substring(1);

            // Check for year suffix /YY
            int slashIdx = rest.IndexOf('/');
            string yearPart = "";
            if (slashIdx >= 0)
            {
                yearPart = rest.Substring(slashIdx + 1);
                rest = rest.Substring(0, slashIdx);
            }

            // Check for day offset .N
            int isoDotIdx = rest.IndexOf('.');
            string weekPart = rest;
            if (isoDotIdx >= 0)
            {
                weekPart = rest.Substring(0, isoDotIdx);
                string dayPart = rest.Substring(isoDotIdx + 1);
                if (int.TryParse(dayPart, out int parsedDay))
                {
                    // Validate day offset is within range 1-7 (Monday-Sunday)
                    if (parsedDay < 1 || parsedDay > 7)
                        return null;
                    isoDayOffset = parsedDay;
                }
            }

            if (int.TryParse(weekPart, out int weekNumber))
            {
                if (!string.IsNullOrEmpty(yearPart))
                {
                    if (int.TryParse(yearPart, out int parsedYear))
                    {
                        year = parsedYear >= 100 ? parsedYear : 2000 + parsedYear;
                    }
                }
                // isoDayOffset is 1-based: 1=Monday, 2=Tuesday, etc. (0 means no offset, default to Monday)
                return GetMondayOfIsoWeek(year, weekNumber).AddDays(isoDayOffset > 0 ? isoDayOffset - 1 : 0);
            }
        }

        // Fallback: try general date parsing using the culture passed by the framework
        try
        {
            DateTimeFormatInfo dtfi = culture.DateTimeFormat;
            if (DateTime.TryParse(text, dtfi, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }
        }
        catch
        {
            // Ignore parsing errors and return null
        }

        return null;
    }

    static DateTime GetMondayOfIsoWeek(int year, int weekNumber)
    {
        // January 4th is always in ISO week 1
        DateTime jan4 = new DateTime(year, 1, 4);
        DayOfWeek dow = jan4.DayOfWeek;
        // Find Monday of week 1
        int daysToMonday = ((int)dow + 6) % 7; // distance back to Monday
        DateTime mondayWeek1 = jan4.AddDays(-daysToMonday);
        // Add weeks to reach the target week
        return mondayWeek1.AddDays((weekNumber - 1) * 7);
    }

}