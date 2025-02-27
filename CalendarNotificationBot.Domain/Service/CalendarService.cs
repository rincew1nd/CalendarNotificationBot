using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using CalendarNotificationBot.Domain.Models.Calendar;
using CalendarNotificationBot.Domain.Service.Interfaces;
using CalendarNotificationBot.Infrastructure.DateTimeProvider;
using Ical.Net.CalendarComponents;
using Microsoft.Extensions.Logging;

namespace CalendarNotificationBot.Domain.Service;

/// <summary>
/// Calendar service.
/// </summary>
public class CalendarService : ICalendarService
{
    /// <summary>
    /// Logger.
    /// </summary>
    private readonly ILogger<CalendarService> _logger;

    /// <summary>
    /// DateTime provider.
    /// </summary>
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>
    /// User calendars.
    /// </summary>
    private readonly ConcurrentDictionary<Guid, CalendarUserData> _userCalendars;
    
    /// <summary>
    /// .ctor
    /// </summary>
    public CalendarService(ILogger<CalendarService> logger, IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _userCalendars = new ConcurrentDictionary<Guid, CalendarUserData>();
    }

    #region Calendar and event update

    /// <inheritdoc/>
    public Dictionary<Guid, Exception> UpdateCalendars(Dictionary<Guid, string> calendars)
    {
        var calendarsToUpdate = new List<Guid>();
        var updateErrors = new Dictionary<Guid, Exception>();
        
        foreach (var calendar in calendars)
        {
            try
            {
                var calendarHashCode = GetCalendarHashCode(calendar.Value);
            
                // If calendar was not updated, ignore it.
                if (_userCalendars.TryGetValue(calendar.Key, out var existingCalendar)
                    && existingCalendar.HashCode == calendarHashCode)
                {
                    continue;
                }
            
                var calendarObj = ICalCalendar.Load(calendar.Value);
                var newCalendarUserData = new CalendarUserData(calendar.Key, calendarObj, calendarHashCode);
            
                _userCalendars[calendar.Key] = newCalendarUserData;
                calendarsToUpdate.Add(calendar.Key);
            
                _logger.LogInformation("New calendar user {UserGuid}", calendar.Key);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during calendar update '{UserId}'", calendar.Key);
                updateErrors.Add(calendar.Key, e);
            }
        }

        if (calendarsToUpdate.Any())
        {
            UpdateUpcomingEvents(calendarsToUpdate);
        }

        return updateErrors;
    }
    
    /// <inheritdoc/>
    public void UpdateUpcomingEvents(ICollection<Guid>? calendarsToUpdate = null)
    {
        // Get all events for next 2 days.
        var endDate = _dateTimeProvider.Today.AddDays(2).AddSeconds(-1);

        var calendars = _userCalendars.Where(uc => calendarsToUpdate?.Contains(uc.Key) ?? true);
        foreach (var calendar in calendars)
        {
            var occurrences = calendar.Value.Calendar.GetOccurrences(_dateTimeProvider.Today, endDate);
            calendar.Value.UpcomingEvents = occurrences
                .Where(o => o.Source is CalendarEvent)
                .Select(o => new CalendarEventLocal(o))
                .DistinctBy(o => o.GetHashCode())
                .ToArray();
        }
    }

    /// <inheritdoc/>
    public bool CalendarExists(Guid userId)
    {
        return _userCalendars.ContainsKey(userId);
    }

    /// <summary>
    /// Calculate calendar hash code.
    /// </summary>
    private int GetCalendarHashCode(string fileContent)
    {
        //It's necessary to remove UID from calendar, because it's always changes.
        return Regex.Replace(fileContent, "UID\\:.+", "").GetHashCode();
    }
    
    #endregion

    #region Fetch events
    
    /// <inheritdoc/>
    public IEnumerable<CalendarEventLocal> GetTodayNotificationsForUser(Guid userId)
    {
        if (_userCalendars.TryGetValue(userId, out var calendarUserData))
        {
            return calendarUserData.UpcomingEvents;
        }
        return Enumerable.Empty<CalendarEventLocal>();
    }

    /// <inheritdoc/>
    public Dictionary<Guid, HashSet<CalendarEventLocal>> GetUpcomingNotificationsForEveryone(DateTime to)
    {
        return GetUpcomingNotificationsForUsers(_userCalendars.Keys, to);
    }

    /// <inheritdoc/>
    public HashSet<CalendarEventLocal>? GetUpcomingNotificationsForUser(Guid userId, DateTime to)
    {
        var notifications = GetUpcomingNotificationsForUsers(new []{ userId }, to);
        notifications.TryGetValue(userId, out var userNotifications);
        return userNotifications;
    }

    /// <summary>
    /// Get upcoming events within the specified dates for a specific user.
    /// </summary>
    /// <param name="userIds">User identifiers</param>
    /// <param name="to">End date for event search in UTC</param>
    /// <returns>List of upcoming events for a specific user</returns>
    private Dictionary<Guid, HashSet<CalendarEventLocal>> GetUpcomingNotificationsForUsers(
        ICollection<Guid> userIds, DateTime to)
    {
        return _userCalendars
            .Where(uc => userIds.Contains(uc.Key))
            .ToDictionary(
                uc => uc.Key,
                uc => uc.Value
                    .UpcomingEvents
                    .Where(e => e.StartTime.Subtract(_dateTimeProvider.UtcNow).TotalSeconds >= 0
                                && to.Subtract(e.StartTime).TotalSeconds >= 0
                                && !e.HasBeenSent)
                    .ToHashSet()
            );
    }

    #endregion
}