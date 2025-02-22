using CalendarNotificationBot.Data.Entities;

namespace CalendarNotificationBot.Data.Repositories.Interfaces;

/// <summary>
/// User repository interface.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Get all users.
    /// </summary>
    Task<IEnumerable<User>> GetAllAsync();
    
    /// <summary>
    /// Get user by unique identifier.
    /// </summary>
    /// <param name="id">Identifier</param>
    Task<User?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Get user by chat identifier.
    /// </summary>
    /// <param name="chatId">Telegram chat identifier</param>
    Task<User?> GetByChatIdAsync(long? chatId);
    
    /// <summary>
    /// Get users with calendar.
    /// </summary>
    Task<IEnumerable<User>> GetUsersWithCalendarAsync();
    
    /// <summary>
    /// Create a new user.
    /// </summary>
    /// <param name="user"><see cref="User"/></param>
    Task CreateAsync(User user);
    
    /// <summary>
    /// Update details.
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <param name="chatId">Telegram chat identifier</param>
    /// <param name="userName">Username</param>
    /// <param name="firstName">First name</param>
    /// <param name="lastName">Last name</param>
    Task UpdateAsync(Guid id, long chatId, string? userName, string? firstName, string? lastName);
    
    /// <summary>
    /// Update time zone.
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <param name="timeZone">Time zone offset</param>
    Task UpdateTimeZoneAsync(Guid id, int timeZone);

    /// <summary>
    /// Update culture.
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <param name="culture">Culture info string</param>
    Task UpdateCultureAsync(Guid id, string? culture);
    
    /// <summary>
    /// Update user's notification time.
    /// </summary>
    Task UpdateNotificationTime(Guid id, short notificationTime);
    
    /// <summary>
    /// Delete user by unique identifier.
    /// </summary>
    /// <param name="id">Identifier</param>
    Task DeleteAsync(Guid id);
}