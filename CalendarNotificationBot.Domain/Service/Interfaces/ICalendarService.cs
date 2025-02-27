using CalendarNotificationBot.Domain.Models.Calendar;

namespace CalendarNotificationBot.Domain.Service.Interfaces;

/// <summary>
/// Calendar service interface.
/// </summary>
public interface ICalendarService
{
    /// <summary>
    /// Update user calendars.
    /// </summary>
    /// <param name="calendars">Calendars for users</param>
    Dictionary<Guid, Exception> UpdateCalendars(Dictionary<Guid, string> calendars);

    /// <summary>
    /// Update calendars.
    /// </summary>
    /// <param name="calendarsToUpdate">User identifiers to update. NULL - all users</param>
    void UpdateUpcomingEvents(ICollection<Guid>? calendarsToUpdate = null);
    
    /// <summary>
    /// Get upcoming events within the specified dates for all users.
    /// </summary>
    /// <param name="to">End date for event search in UTC</param>
    /// <returns>List of upcoming events for all users</returns>
    Dictionary<Guid, HashSet<CalendarEventLocal>> GetUpcomingNotificationsForEveryone(DateTime to);

    /// <summary>
    /// Get upcoming events within the specified dates for a specific user.
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="to">End date for event search in UTC</param>
    /// <returns>List of upcoming events for a specific user</returns>
    HashSet<CalendarEventLocal>? GetUpcomingNotificationsForUser(Guid userId, DateTime to);

    /// <summary>
    /// Get today's events for a specific user.
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>List of today's events for a specific user</returns>
    IEnumerable<CalendarEventLocal> GetTodayNotificationsForUser(Guid userId);

    /// <summary>
    /// Check if user calendar exists.
    /// </summary>
    /// <param name="userId">User identifier</param>
    bool CalendarExists(Guid userId);
}