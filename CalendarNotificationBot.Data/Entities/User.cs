namespace CalendarNotificationBot.Data.Entities;

/// <summary>
/// Telegram user data.
/// </summary>
public class User
{
    /// <summary>
    /// Identifier.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Telegram chat identifier.
    /// </summary>
    public long ChatId { get; set; }
    
    /// <summary>
    /// Username.
    /// </summary>
    public string? Username  { get; set; }
    
    /// <summary>
    /// First name.
    /// </summary>
    public string? Firstname { get; set; }
    
    /// <summary>
    /// Last name.
    /// </summary>
    public string? Lastname { get; set; }
    
    /// <summary>
    /// Timezone (currently a number for GMT timezone).
    /// </summary>
    /// <remarks>
    /// For example +3 for Moscow, -5 for New York.
    /// </remarks>
    public int TimeZone { get; set; }
    
    /// <summary>
    /// User's culture info.
    /// </summary>
    public string Culture { get; set; }
    
    /// <summary>
    /// Время оповещения о событии (в минутах).
    /// </summary>
    public int NotificationTime { get; set; }
}