namespace CalendarNotificationBot.Domain.Service.Telegram;

/// <summary>
/// Possible user states.
/// </summary>
public enum UserState
{
    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown = 0,
    
    /// <summary>
    /// In main menu.
    /// </summary>
    MainMenu = 1,
    
    /// <summary>
    /// Updating contact data.
    /// </summary>
    UpdateContactData = 2,
    
    /// <summary>
    /// Sharing contact data from telegram account.
    /// </summary>
    ShareContactData = 3,
    
    /// <summary>
    /// Viewing profile.
    /// </summary>
    ViewInfo = 4,
    
    /// <summary>
    /// Updating calendar.
    /// </summary>
    UpdateCalendar = 5,
    
    /// <summary>
    /// Removing calendar.
    /// </summary>
    RemoveCalendar = 6,
    
    /// <summary>
    /// Updating time zone.
    /// </summary>
    UpdateTimeZone = 7,
    
    /// <summary>
    /// Register user.
    /// </summary>
    Register = 8,
    
    /// <summary>
    /// Update user's culture info.
    /// </summary>
    UpdateCulture = 9,
    
    /// <summary>
    /// Update user's notification time.
    /// </summary>
    UpdateNotificationTime = 10,
}