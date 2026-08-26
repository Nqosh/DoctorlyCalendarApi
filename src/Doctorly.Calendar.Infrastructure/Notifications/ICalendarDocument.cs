using System.Globalization;

namespace Doctorly.Calendar.Infrastructure.Notifications;

public static class ICalendarDocument
{
    public static string Create(
        Guid eventId,
        string title,
        DateTimeOffset start,
        DateTimeOffset end)
    {
        return string.Join(
            "\r\n",
            "BEGIN:VCALENDAR",
            "VERSION:2.0",
            "PRODID:-//Doctorly//Calendar API//EN",
            "METHOD:REQUEST",
            "BEGIN:VEVENT",
            $"UID:{eventId}@doctorly.local",
            $"DTSTAMP:{Format(DateTimeOffset.UtcNow)}",
            $"DTSTART:{Format(start)}",
            $"DTEND:{Format(end)}",
            $"SUMMARY:{Escape(title)}",
            "END:VEVENT",
            "END:VCALENDAR",
            string.Empty);
    }

    private static string Format(
        DateTimeOffset value)
    {
        return value.UtcDateTime.ToString(
            "yyyyMMdd'T'HHmmss'Z'",
            CultureInfo.InvariantCulture);
    }

    private static string Escape(
        string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace(",", "\\,")
            .Replace(";", "\\;")
            .Replace("\n", "\\n");
    }
}