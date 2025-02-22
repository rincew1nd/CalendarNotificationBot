using CalendarNotificationBot.Data.Repositories.Interfaces;
using CalendarNotificationBot.Domain.Resources;
using CalendarNotificationBot.Domain.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CalendarNotificationBot.Domain.Service.Telegram.Handlers;

/// <summary>
/// Remove user's calendar command.
/// </summary>
/// <param name="Message"></param>
public record RemoveCalendarCommand(Message Message) : IRequest<UserState?>;

/// <summary>
/// Remove user's calendar.
/// </summary>
public class RemoveCalendarHandler : IRequestHandler<RemoveCalendarCommand, UserState?>
{
    /// <summary>
    /// Telegram bot client.
    /// </summary>
    private readonly ITelegramBotClient _botClient;

    /// <summary>
    /// User repository.
    /// </summary>
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Calendar repository.
    /// </summary>
    private readonly ICalendarRepository _calendarRepository;

    /// <summary>
    /// User service.
    /// </summary>
    private readonly IUserService _userService;

    /// <summary>
    /// Strings localization provider.
    /// </summary>
    private readonly IStringLocalizer _localizationProvider;

    /// <summary>
    /// .ctor
    /// </summary>
    public RemoveCalendarHandler(
        ITelegramBotClient botClient,
        IUserRepository userRepository,
        ICalendarRepository calendarRepository,
        IUserService userService,
        IStringLocalizer<SharedResource> localizationProvider)
    {
        _botClient = botClient;
        _userRepository = userRepository;
        _calendarRepository = calendarRepository;
        _userService = userService;
        _localizationProvider = localizationProvider;
    }
    
    /// <summary>
    /// Handle.
    /// </summary>
    public async Task<UserState?> Handle(RemoveCalendarCommand request, CancellationToken cancellationToken)
    {
        var result = false;
        
        var user = await _userRepository.GetByChatIdAsync(request.Message!.Chat.Id);

        if (user != null)
        {
            var calendar = await _calendarRepository.GetByUserIdAsync(user.Id);
            
            if (calendar != null)
            {
                await _calendarRepository.DeleteAsync(calendar.UserId);
                result = true;
            }
        }
        
        await _botClient.SendMessage(
            chatId: request.Message.Chat.Id,
            text: result ? _localizationProvider["CalendarDeleted_Message"] : _localizationProvider["CalendarNotDeleted_Message"],
            cancellationToken: cancellationToken);
        
        _userService.UpdateUserState(request.Message.Chat.Id, UserState.MainMenu);

        return UserState.MainMenu;
    }
}