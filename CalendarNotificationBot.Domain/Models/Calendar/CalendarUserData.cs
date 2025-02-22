namespace CalendarNotificationBot.Domain.Models.Calendar;

/// <summary>
/// User's calendar information.
/// </summary>
internal class CalendarUserData
{
    /// <summary>
    /// .ctor
    /// </summary>
    /// <param name="userId">User's identifier</param>
    /// <param name="calendar"><see cref="ICalCalendar"/></param>
    /// <param name="hashCode">Calendar file's hash code</param>
    public CalendarUserData(Guid userId, ICalCalendar calendar, int hashCode)
    {
        UserGuid = userId;
        Calendar = calendar;
        HashCode = hashCode;

        UpcomingEvents = Array.Empty<CalendarEventLocal>();
    }

    /// <summary>
    /// User's identifier.
    /// </summary>
    public Guid UserGuid { get; set; }

    /// <summary>
    /// Calendar data.
    /// </summary>
    public Ical.Net.Calendar Calendar { get; set; }

    /// <summary>
    /// Upcoming calendar events for today.
    /// </summary>
    public CalendarEventLocal[] UpcomingEvents { get; set; }

    /// <summary>
    /// Calendar hash code.
    /// </summary>
    public int HashCode { get; set; }
}