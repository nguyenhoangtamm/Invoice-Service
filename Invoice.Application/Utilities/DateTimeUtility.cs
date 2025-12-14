namespace Invoice.Application.Utilities;

/// <summary>
/// Utility class for handling DateTime conversions and validations
/// Ensures PostgreSQL compatibility by converting to UTC
/// </summary>
public static class DateTimeUtility
{
    /// <summary>
    /// Converts a DateTime string to UTC DateTime
    /// </summary>
    /// <param name="dateString">The date string to parse</param>
    /// <param name="utcDateTime">The parsed UTC DateTime</param>
    /// <returns>True if parsing succeeded and conversion was done, false otherwise</returns>
    public static bool TryParseToUtc(string? dateString, out DateTime utcDateTime)
    {
        utcDateTime = DateTime.MinValue;

        if (string.IsNullOrWhiteSpace(dateString))
            return false;

        if (!DateTime.TryParse(dateString, out var parsedDateTime))
            return false;

        // Ensure UTC datetime for PostgreSQL compatibility
        utcDateTime = parsedDateTime.Kind == DateTimeKind.Local
            ? parsedDateTime.ToUniversalTime()
            : parsedDateTime;

        return true;
    }

    /// <summary>
    /// Gets the end of the specified day in UTC (23:59:59.9999999)
    /// Useful for inclusive date range queries
    /// </summary>
    /// <param name="date">The date to get the end of</param>
    /// <returns>DateTime representing the end of the day in UTC</returns>
    public static DateTime GetEndOfDayUtc(DateTime date)
    {
        // Ensure the input is UTC
        var utcDate = date.Kind == DateTimeKind.Local
            ? date.ToUniversalTime()
            : date;

        // Return the end of the day
        return utcDate.AddDays(1).AddTicks(-1);
    }
}
