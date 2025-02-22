using CalendarNotificationBot.Domain.Resources;
using MediatR;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CalendarNotificationBot.Domain.Service.Telegram.Handlers;

/// <summary>
/// Command to send main menu to telegram chat.
/// </summary>
/// <param name="Message">Incoming message</param>
public record MainMenuCommand(Message Message) : IRequest<UserState?>;

/// <summary>
/// Send main menu to telegram chat.
/// </summary>
public class MainMenuHandler : IRequestHandler<MainMenuCommand, UserState?>
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
    public MainMenuHandler(ITelegramBotClient botClient, IStringLocalizer<SharedResource> localizationProvider)
    {
        _botClient = botClient;
        _localizationProvider = localizationProvider;
    }

    /// <summary>
    /// Handle.
    /// </summary>
    public async Task<UserState?> Handle(MainMenuCommand request, CancellationToken cancellationToken)
    {
        InlineKeyboardMarkup inlineKeyboard = new(
            new InlineKeyboardButton[][]
            {
                [
                    InlineKeyboardButton.WithCallbackData(_localizationProvider["UpdateCalendar_Menu"],
                        UserState.UpdateCalendar.ToString()),
                    InlineKeyboardButton.WithCallbackData(_localizationProvider["DeleteCalendar_Menu"],
                        UserState.RemoveCalendar.ToString())
                ],
                [
                    InlineKeyboardButton.WithCallbackData(_localizationProvider["UpdateContactData_Menu"],
                        UserState.UpdateContactData.ToString())
                ],
                [
                    InlineKeyboardButton.WithCallbackData(_localizationProvider["UpdateContactDataFromAccount_Menu"],
                        UserState.ShareContactData.ToString())
                ],
                [
                    InlineKeyboardButton.WithCallbackData(_localizationProvider["ChangeTimezone_Menu"],
                        UserState.UpdateTimeZone.ToString()),
                    InlineKeyboardButton.WithCallbackData(_localizationProvider["UpdateNotificationTime_Menu"],
                        UserState.UpdateNotificationTime.ToString())
                ],
                [
                    InlineKeyboardButton.WithCallbackData(_localizationProvider["Information_Menu"],
                        UserState.ViewInfo.ToString()),
                    InlineKeyboardButton.WithCallbackData("\ud83c\uddfa\ud83c\uddf8",
                        $"{UserState.UpdateCulture};en-US"),
                    InlineKeyboardButton.WithCallbackData("\ud83c\uddf7\ud83c\uddfa",
                        $"{UserState.UpdateCulture};ru-RU")
                ]
            });

        await _botClient.SendMessage(
            chatId: request.Message.Chat.Id,
            text: _localizationProvider["ChooseAction_Menu"],
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
        
        return null;
    }
}