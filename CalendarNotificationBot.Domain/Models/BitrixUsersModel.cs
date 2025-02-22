namespace CalendarNotificationBot.Domain.Models;

/// <summary>
/// Information about Bitrix users.
/// </summary>
public class BitrixUsersModel
{
    /// <summary>
    /// Bitrix user's identifiers.
    /// </summary>
    public string[] BitrixUserIds { get; init; } = Array.Empty<string>();
}