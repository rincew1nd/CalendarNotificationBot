using System.Collections.Concurrent;
using System.Globalization;
using CalendarNotificationBot.Domain.Service.Interfaces;
using CalendarNotificationBot.Domain.Service.Telegram;

namespace CalendarNotificationBot.Domain.Service;

/// <summary>
/// User service.
/// </summary>
public class UserService : IUserService
{
    /// <summary>
    /// Current user statuses.
    /// </summary>
    private static readonly ConcurrentDictionary<long, UserData> _userStates = new();

    /// <inheritdoc/>
    public UserState GetUserState(long telegramUserId)
    {
        if (!_userStates.ContainsKey(telegramUserId))
        {
            _userStates.TryAdd(telegramUserId, UserData.DefaultUserData);
        }
        return _userStates[telegramUserId].UserState;
    }
    
    /// <inheritdoc/>
    public void UpdateUserState(long telegramUserId, UserState userState)
    {
        if (!_userStates.ContainsKey(telegramUserId))
        {
            _userStates.TryAdd(telegramUserId, UserData.DefaultUserData);
        }
        else
        {
            _userStates[telegramUserId].UserState = userState;
        }
    }

    public string GetCultureInfo(long telegramUserId)
    {
        if (!_userStates.ContainsKey(telegramUserId))
        {
            _userStates.TryAdd(telegramUserId, UserData.DefaultUserData);
        }
        return _userStates[telegramUserId].CultureInfo;
    }
    
    public void SetCultureInfo(long telegramUserId, string cultureInfo)
    {
        if (!_userStates.ContainsKey(telegramUserId))
        {
            _userStates.TryAdd(telegramUserId, UserData.DefaultUserData);
        }
        _userStates[telegramUserId].CultureInfo = cultureInfo;
    }

    class UserData
    {
        public UserState UserState { get; set; }
        public string CultureInfo { get; set; }

        public static UserData DefaultUserData => new()
        {
            UserState = UserState.Register,
            CultureInfo = "en-US"
        };
    }
}