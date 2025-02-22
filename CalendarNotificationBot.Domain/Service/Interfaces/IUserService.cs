using System.Globalization;
using CalendarNotificationBot.Domain.Service.Telegram;

namespace CalendarNotificationBot.Domain.Service.Interfaces;

/// <summary>
/// User service interface.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Get current user state.
    /// </summary>
    /// <param name="telegramUserId">Telegram user identifier</param>
    UserState GetUserState(long telegramUserId);

    /// <summary>
    /// Set user state.
    /// </summary>
    /// <param name="telegramUserId">Telegram user identifier</param>
    /// <param name="userState"><see cref="UserState"/></param>
    void UpdateUserState(long telegramUserId, UserState userState);
    
    /// <summary>
    /// Get user culture info. 
    /// </summary>
    /// <param name="telegramUserId"></param>
    string GetCultureInfo(long telegramUserId);

    /// <summary>
    /// Set user culture info.
    /// </summary>
    void SetCultureInfo(long telegramUserId, string cultureInfo);
}