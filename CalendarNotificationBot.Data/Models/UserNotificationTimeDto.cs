namespace CalendarNotificationBot.Data.Models;

/// <summary>
/// User notification time (with filter for existing calendar).
/// </summary>
public class UserNotificationTimeDto
{
    /// <summary>
    /// Identifier.
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Время оповещения о событии (в минутах).
    /// </summary>
    public int NotificationTime { get; set; }
}