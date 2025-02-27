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
public record MainMenuCommand(Message Message, CallbackQuery? CallbackQuery) : IRequest<UserState?>;

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
        Enum.TryParse(request.CallbackQuery?.Data, out MenuState menuState);
        var inlineKeyboard = menuState switch
        {
            MenuState.MenuCalendar => BuildCalendarMenu(),
            MenuState.MenuContacts => BuildContactsMenu(),
            MenuState.MenuSettings => BuildSettingsMenu(),
            _ => BuildMainMenu(),
        };

        if (request.CallbackQuery == null)
        {
            await _botClient.SendMessage(
                request.Message.Chat.Id,
                text: _localizationProvider["ChooseAction_Menu"],
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
        else
        {
            await _botClient.EditMessageReplyMarkup(
                request.Message.Chat.Id,
                request.Message.MessageId,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
            await _botClient.AnswerCallbackQuery(
                request.CallbackQuery.Id,
                cancellationToken: cancellationToken);
        }

        return null;
    }

    // Main menu
    private InlineKeyboardMarkup BuildMainMenu()
    {
        return new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData(
                    _localizationProvider["CalendarButton_Menu"],
                    MenuState.MenuCalendar.ToString()
                ),
                InlineKeyboardButton.WithCallbackData(
                    _localizationProvider["ContactsButton_Menu"],
                    MenuState.MenuContacts.ToString()
                ),
                InlineKeyboardButton.WithCallbackData(
                    _localizationProvider["SettingsButton_Menu"],
                    MenuState.MenuSettings.ToString()
                )
            ],
            [
                InlineKeyboardButton.WithCallbackData(
                    _localizationProvider["Information_Menu"],
                    UserState.ViewInfo.ToString())
            ]
        ]);
    }

    // Calendar submenu
    private InlineKeyboardMarkup BuildCalendarMenu()
    {
        return new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData(
                    _localizationProvider["ChangeCalendarFile_Menu"],
                    UserState.ChangeCalendarFile.ToString()),
                InlineKeyboardButton.WithCallbackData(
                    _localizationProvider["DeleteCalendar_Menu"],
                    UserState.RemoveCalendar.ToString())
            ],
            [
                InlineKeyboardButton.WithCallbackData(
                    _localizationProvider["RedownloadCalendar_Menu"],
                    UserState.RedownloadCalendar.ToString()),
                InlineKeyboardButton.WithCallbackData(
                    _localizationProvider["UpcomingEvents_Menu"],
                    UserState.UpcomingEvents.ToString()),
            ],
            [
                InlineKeyboardButton.WithCallbackData(
                    _localizationProvider["BackButton_Menu"],
                    MenuState.MenuMain.ToString()
                )
            ]
        ]);
    }

    // Contacts submenu
    private InlineKeyboardMarkup BuildContactsMenu()
    {
        return new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData(
                    _localizationProvider["UpdateContactData_Menu"],
                    UserState.UpdateContactData.ToString()
                )
            ],
            [
                InlineKeyboardButton.WithCallbackData(
                    _localizationProvider["UpdateContactDataFromAccount_Menu"],
                    UserState.ShareContactData.ToString()
                )
            ],
            [
                InlineKeyboardButton.WithCallbackData(
                    _localizationProvider["BackButton_Menu"],
                    MenuState.MenuMain.ToString()
                )
            ]
        ]);
    }

    // Settings submenu
    private InlineKeyboardMarkup BuildSettingsMenu()
    {
        return new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData(_localizationProvider["ChangeTimezone_Menu"],
                    UserState.UpdateTimeZone.ToString()),
                InlineKeyboardButton.WithCallbackData(_localizationProvider["UpdateNotificationTime_Menu"],
                    UserState.UpdateNotificationTime.ToString())
            ],
            [
                InlineKeyboardButton.WithCallbackData("\ud83c\uddfa\ud83c\uddf8",
                    $"{UserState.UpdateCulture};en-US"),
                InlineKeyboardButton.WithCallbackData("\ud83c\uddf7\ud83c\uddfa",
                    $"{UserState.UpdateCulture};ru-RU")
            ],
            [
                InlineKeyboardButton.WithCallbackData(
                    _localizationProvider["BackButton_Menu"],
                    MenuState.MenuMain.ToString()
                )
            ]
        ]);
    }

    enum MenuState
    {
        MenuMain = 0,
        MenuCalendar = 1,
        MenuContacts = 2,
        MenuSettings = 3
    }
}