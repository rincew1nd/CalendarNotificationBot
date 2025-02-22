using CalendarNotificationBot.Data.Repositories.Interfaces;
using CalendarNotificationBot.Domain.Extensions;
using CalendarNotificationBot.Domain.Resources;
using CalendarNotificationBot.Domain.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CalendarNotificationBot.Domain.Service.Telegram.Handlers;

/// <summary>
/// View user and calendar information command.
/// </summary>
public record ViewInfoCommand(Message Message) : IRequest<UserState?>;

/// <summary>
/// View user and calendar information.
/// </summary>
public class ViewInfoHandler : IRequestHandler<ViewInfoCommand, UserState?>
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
    public ViewInfoHandler(
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
    public async Task<UserState?> Handle(ViewInfoCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByChatIdAsync(request.Message!.Chat.Id);
        var calendar = user?.Id == null ? null : await _calendarRepository.GetByUserIdAsync(user.Id);
        
        await _botClient.SendMessage(
            chatId: request.Message.Chat.Id,
            text: string.Format(
                _localizationProvider["UserInfo_Message"],
                user?.Username,
                user?.Firstname,
                user?.Lastname,
                user.Id.ToString().EscapeStringForMarkdown(),
                user.ChatId,
                string.IsNullOrEmpty(calendar?.CalendarUrl)
                    ? _localizationProvider["CalendarNotFound_Message"]
                    : $"[*{_localizationProvider["CalendarLink_Message"]}*]({calendar?.CalendarUrl})",
                $"GMT{(user.TimeZone >= 0 ? "\\+" : "\\-")}{Math.Abs(user.TimeZone)}",
                user.Culture.Replace("-", "\\-"),
                user.NotificationTime
            ),
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);

        _userService.UpdateUserState(request.Message.Chat.Id, UserState.MainMenu);

        return UserState.MainMenu;
    }
}
