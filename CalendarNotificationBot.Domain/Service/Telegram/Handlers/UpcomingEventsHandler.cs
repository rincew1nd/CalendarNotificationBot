using CalendarNotificationBot.Data.Repositories.Interfaces;
using CalendarNotificationBot.Domain.Extensions;
using CalendarNotificationBot.Domain.Resources;
using CalendarNotificationBot.Domain.Service.Interfaces;
using CalendarNotificationBot.Infrastructure.DateTimeProvider;
using MediatR;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CalendarNotificationBot.Domain.Service.Telegram.Handlers;

/// <summary>
/// Command to get upcoming events.
/// </summary>
/// <param name="Message">Incoming message</param>
public record UpcomingEventsCommand(Message Message) : IRequest<UserState?>;

/// <summary>
/// Get upcoming events.
/// </summary>
public class UpcomingEventsHandler : IRequestHandler<UpcomingEventsCommand, UserState?>
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
    /// User service.
    /// </summary>
    private readonly IUserService _userService;
    
    /// <summary>
    /// Calendar service.
    /// </summary>
    private readonly ICalendarService _calendarService;

    /// <summary>
    /// DateTime provider.
    /// </summary>
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>
    /// Strings localization provider.
    /// </summary>
    private readonly IStringLocalizer _localizationProvider;

    /// <summary>
    /// .ctor
    /// </summary>
    public UpcomingEventsHandler(
        ITelegramBotClient botClient,
        IUserRepository userRepository,
        IUserService userService,
        ICalendarService calendarService,
        IDateTimeProvider dateTimeProvider,
        IStringLocalizer<SharedResource> localizationProvider)
    {
        _botClient = botClient;
        _userRepository = userRepository;
        _userService = userService;
        _calendarService = calendarService;
        _dateTimeProvider = dateTimeProvider;
        _localizationProvider = localizationProvider;
    }

    /// <summary>
    /// Handle.
    /// </summary>
    public async Task<UserState?> Handle(UpcomingEventsCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByChatIdAsync(request.Message.Chat.Id);
        
        if (user == null)
        {
            await _botClient.SendMessage(
                chatId: request.Message.Chat.Id,
                text: _localizationProvider["UserNotFound_Message"],
                cancellationToken: cancellationToken);
            return null;
        }

        var upcomingEvents =
            _calendarService.GetUpcomingNotificationsForUser(user.Id, _dateTimeProvider.Today.AddDays(2));

        var message = (upcomingEvents?.Count ?? 0) == 0
            ? _localizationProvider["UpcomingEventsNotFound_Message"]
            : $"<b>{_localizationProvider["UpcomingEvents_Message"]}</b>\n\n{
              string.Join(
                "\n----------\n",
                (upcomingEvents ?? [])
                    .Select(ue =>
                        string.Format(
                            _localizationProvider["CalendarEventShort_Message"],
                            ue.Summary.EscapeStringForHtml(),
                            ue.StartTime.ToUserTimeZone(user).ToString("dd-MM-yyyy HH:mm:ss")
            )))}";
        
        await _botClient.SendMessage(
            chatId: request.Message.Chat.Id,
            text: message,
            ParseMode.Html,
            cancellationToken: cancellationToken);
        
        _userService.UpdateUserState(user.ChatId, UserState.MainMenu);
        return null;
    }
}