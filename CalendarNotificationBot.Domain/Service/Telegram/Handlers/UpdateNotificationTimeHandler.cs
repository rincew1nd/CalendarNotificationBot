using CalendarNotificationBot.Data.Repositories.Interfaces;
using CalendarNotificationBot.Domain.Resources;
using MediatR;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CalendarNotificationBot.Domain.Service.Telegram.Handlers;

/// <summary>
/// Update contact data command.
/// </summary>
/// <param name="Message"></param>
public record UpdateNotificationTimeCommand(Message Message) : IRequest<UserState?>;

/// <summary>
/// Update contact data.
/// </summary>
public class UpdateNotificationTimeHandler : IRequestHandler<UpdateNotificationTimeCommand, UserState?>
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
    /// Strings localization provider.
    /// </summary>
    private readonly IStringLocalizer _localizationProvider;

    /// <summary>
    /// .ctor
    /// </summary>
    public UpdateNotificationTimeHandler(
        ITelegramBotClient botClient,
        IUserRepository userRepository,
        IStringLocalizer<SharedResource> localizationProvider)
    {
        _botClient = botClient;
        _userRepository = userRepository;
        _localizationProvider = localizationProvider;
    }
    
    /// <summary>
    /// Handle.
    /// </summary>
    public async Task<UserState?> Handle(UpdateNotificationTimeCommand request, CancellationToken cancellationToken)
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
        
        if (request.Message.ReplyMarkup != null)
        {
            await _botClient.SendMessage(
                chatId: request.Message.Chat.Id,
                text: _localizationProvider["UpdateNotificationTime_Message"],
                cancellationToken: cancellationToken);
            return null;
        }

        if (!short.TryParse(request.Message.Text, out var notificationTime) || notificationTime is <2 or >1440)
        {
            await _botClient.SendMessage(
                chatId: request.Message.Chat.Id,
                text: _localizationProvider["IncorrectNotificationTime_Message"],
                cancellationToken: cancellationToken);
            return null;
        }
        
        await _userRepository.UpdateNotificationTime(user.Id, notificationTime);

        return UserState.MainMenu;
    }
}