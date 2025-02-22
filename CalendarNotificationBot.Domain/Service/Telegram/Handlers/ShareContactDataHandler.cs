using CalendarNotificationBot.Domain.Resources;
using MediatR;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CalendarNotificationBot.Domain.Service.Telegram.Handlers;

/// <summary>
/// Share contact data command.
/// </summary>
/// <param name="Message"></param>
public record ShareContactDataCommand(Message Message) : IRequest<UserState?>;

/// <summary>
/// Share contact data.
/// </summary>
public class ShareContactDataHandler : IRequestHandler<ShareContactDataCommand, UserState?>
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
    /// .ctor
    /// </summary>
    public ShareContactDataHandler(ITelegramBotClient botClient, IStringLocalizer<SharedResource> localizationProvider)
    {
        _botClient = botClient;
        _localizationProvider = localizationProvider;
    }
    
    /// <summary>
    /// Handle.
    /// </summary>
    public async Task<UserState?> Handle(ShareContactDataCommand request, CancellationToken cancellationToken)
    {
        if (request.Message.Contact != null)
        {
            return UserState.UpdateContactData;
        }
        
        ReplyKeyboardMarkup shareContactDataKeyboardMarkup = new(
            new[]
            {
                KeyboardButton.WithRequestContact(_localizationProvider["ContactData_Menu"]),
            });

        await _botClient.SendMessage(
            chatId: request.Message!.Chat.Id,
            text: _localizationProvider["ShareContactData_Message"],
            replyMarkup: shareContactDataKeyboardMarkup,
            cancellationToken: cancellationToken);

        return null;
    }
}