using System.Text.RegularExpressions;
using CalendarNotificationBot.Data.Repositories.Interfaces;
using CalendarNotificationBot.Domain.Resources;
using CalendarNotificationBot.Domain.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CalendarNotificationBot.Domain.Service.Telegram.Handlers;

/// <summary>
/// Update user's time zone command.
/// </summary>
public record UpdateTimezoneCommand(Message Message) : IRequest<UserState?>;

/// <summary>
/// Update user's time zone.
/// </summary>
public partial class UpdateTimezoneHandler : IRequestHandler<UpdateTimezoneCommand, UserState?>
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
    /// Strings localization provider.
    /// </summary>
    private readonly IStringLocalizer _localizationProvider;

    /// <summary>
    /// .ctor
    /// </summary>
    public UpdateTimezoneHandler(
        ITelegramBotClient botClient,
        IUserRepository userRepository,
        IUserService userService,
        IStringLocalizer<SharedResource> localizationProvider)
    {
        _botClient = botClient;
        _userRepository = userRepository;
        _userService = userService;
        _localizationProvider = localizationProvider;
    }
    
    /// <summary>
    /// Handle.
    /// </summary>
    public async Task<UserState?> Handle(UpdateTimezoneCommand request, CancellationToken cancellationToken)
    {
        if (request.Message.ReplyMarkup != null)
        {
            await _botClient.SendMessage(
                chatId: request.Message.Chat.Id,
                text: _localizationProvider["UpdateTimezone_Message"],
                cancellationToken: cancellationToken);
            return null;
        }
        
        var user = await _userRepository.GetByChatIdAsync(request.Message.Chat.Id);

        if (user == null)
        {
            await _botClient.SendMessage(
                chatId: request.Message.Chat.Id,
                text: _localizationProvider["UserNotFound_Message"],
                cancellationToken: cancellationToken);
            return null;
        }

        var matches = TimezoneRegex().Match(request.Message.Text ?? "");
        
        if (!matches.Captures.Any())
        {
            await _botClient.SendMessage(
                chatId: request.Message.Chat.Id,
                text: _localizationProvider["UpdateTimezone_Message"],
                cancellationToken: cancellationToken);
            return null;
        }
        
        var timeZone = int.Parse(matches.Groups[2].Value);

        if (timeZone is < -12 or > 15)
        {
            await _botClient.SendMessage(
                chatId: request.Message.Chat.Id,
                text: _localizationProvider["TimezoneLimit_Message"],
                cancellationToken: cancellationToken);
            return null;
        }
        
        await _userRepository.UpdateTimeZoneAsync(user.Id, timeZone);

        await _botClient.SendMessage(
            chatId: request.Message.Chat.Id,
            text: string.Format(_localizationProvider["TimezoneChanged_Message"], request.Message.Text),
            cancellationToken: cancellationToken);
        
        _userService.UpdateUserState(user.ChatId, UserState.MainMenu);

        return UserState.MainMenu;
    }

    [GeneratedRegex("^(GMT|gmt)([-+]\\d{1,2})$")]
    private static partial Regex TimezoneRegex();
}