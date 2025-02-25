using CalendarNotificationBot.Data.Entities;

namespace CalendarNotificationBot.Data.Repositories.Interfaces;

/// <summary>
/// Calendar repository interface.
/// </summary>
public interface ICalendarRepository
{
    /// <summary>
    /// Get all users.
    /// </summary>
    /// <param name="withoutBitrixUsers">With or without users of Bitrix calendar</param>
    Task<IEnumerable<Calendar>> GetAllAsync(bool withoutBitrixUsers = false);
    
    /// <summary>
    /// Get user by identifier.
    /// </summary>
    /// <param name="id">User's identifier</param>
    Task<Calendar?> GetByUserIdAsync(Guid id);
    
    /// <summary>
    /// Get user by Bitrix identifiers.
    /// </summary>
    /// <param name="bitrixUserIds">User's Bitrix identifiers</param>
    Task<IEnumerable<Calendar>> GetByBitrixUserIdsAsync(string[] bitrixUserIds);
    
    /// <summary>
    /// Get user by identifiers.
    /// </summary>
    /// <param name="userIds">User's identifiers</param>
    Task<IEnumerable<Calendar>> GetByUserIdsAsync(Guid[] userIds);
    
    /// <summary>
    /// Create calendar.
    /// </summary>
    Task CreateAsync(Calendar calendar);
    
    /// <summary>
    /// Update calendar.
    /// </summary>
    Task UpdateAsync(Calendar calendar);
    
    /// <summary>
    /// Delete calendar.
    /// </summary>
    Task DeleteAsync(Guid id);
}