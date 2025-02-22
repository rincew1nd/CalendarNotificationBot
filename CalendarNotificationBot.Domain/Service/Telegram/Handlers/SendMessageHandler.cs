using CalendarNotificationBot.Domain.Resources;
using CalendarNotificationBot.Domain.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CalendarNotificationBot.Domain.Service.Telegram.Handlers;

/// <summary>
/// Command for sending message to user.
/// </summary>
/// <param name="Message">Message from user</param>
/// <param name="UserMessage">Message for user</param>
public record SendMessageCommand(Message Message, string UserMessage, UserState? NextUserState) : IRequest<UserState?>;

/// <summary>
/// Send message to user.
/// </summary>
public class SendMessageHandler : IRequestHandler<SendMessageCommand, UserState?>
{
    /// <summary>
    /// Telegram bot client.
    /// </summary>
    private readonly ITelegramBotClient _botClient;

    /// <summary>
    /// Strings localization provider.
    /// </summary>
    private readonly IStringLocalizer _localizationProvider;

    /// <summary>
    /// User service.
    /// </summary>
    private readonly IUserService _userService;

    /// <summary>
    /// .ctor
    /// </summary>
    public SendMessageHandler(
        ITelegramBotClient botClient,
        IStringLocalizer<SharedResource> localizationProvider,
        IUserService userService)
    {
        _botClient = botClient;
        _localizationProvider = localizationProvider;
        _userService = userService;
    }
    
    /// <summary>
    /// Handle.
    /// </summary>
    public async Task<UserState?> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        await _botClient.SendMessage(
            chatId: request.Message.Chat.Id,
            text: _localizationProvider[request.UserMessage],
            cancellationToken: cancellationToken);

        if (request.NextUserState != null)
        {
            _userService.UpdateUserState(request.Message.Chat.Id, request.NextUserState.Value);
        }
        
        return request.NextUserState;
    }
}