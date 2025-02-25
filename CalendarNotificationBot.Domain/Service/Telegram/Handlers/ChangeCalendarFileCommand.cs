using System.Text.RegularExpressions;
using CalendarNotificationBot.Data.Repositories.Interfaces;
using CalendarNotificationBot.Domain.Resources;
using CalendarNotificationBot.Domain.Service.Interfaces;
using CalendarNotificationBot.Domain.UseCases;
using CalendarNotificationBot.Infrastructure.DateTimeProvider;
using MediatR;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CalendarNotificationBot.Domain.Service.Telegram.Handlers;

/// <summary>
/// Command to change user calendar file.
/// </summary>
public record ChangeCalendarFileCommand(Message Message) : IRequest<UserState?>;

/// <summary>
/// Change user calendar file.
/// </summary>
public partial class ChangeCalendarFileHandler : IRequestHandler<ChangeCalendarFileCommand, UserState?>
{
    /// <summary>
    /// Image path to use as a guide to add bitrix calendar.
    /// </summary>
    private const string BitrixCalendarTutorialImage =
        "BitrixNotificationBot.Domain.Resources.Images.BitrixCalendarGuide.png";
    
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
    /// Calendar repository.
    /// </summary>
    private readonly ICalendarRepository _calendarRepository;

    /// <summary>
    /// Calendar service.
    /// </summary>
    private readonly UpdateCalendarUseCase _updateCalendarUseCase;


    /// <summary>
    /// Strings localization provider.
    /// </summary>
    private readonly IStringLocalizer _localizationProvider;

    /// <summary>
    /// DateTime provider.
    /// </summary>
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>
    /// .ctor
    /// </summary>
    public ChangeCalendarFileHandler(
        ITelegramBotClient botClient,
        IUserRepository userRepository,
        IUserService userService,
        ICalendarRepository calendarRepository,
        UpdateCalendarUseCase updateCalendarUseCase,
        IStringLocalizer<SharedResource> localizationProvider,
        IDateTimeProvider dateTimeProvider)
    {
        _botClient = botClient;
        _userRepository = userRepository;
        _calendarRepository = calendarRepository;
        _updateCalendarUseCase = updateCalendarUseCase;
        _userService = userService;
        _localizationProvider = localizationProvider;
        _dateTimeProvider = dateTimeProvider;
    }
    
    /// <summary>
    /// Handle.
    /// </summary>
    public async Task<UserState?> Handle(ChangeCalendarFileCommand request, CancellationToken cancellationToken)
    {
        if (request.Message.ReplyMarkup != null)
        {
            await _botClient.SendMessage(
                chatId: request.Message.Chat.Id,
                text: _localizationProvider["UpdateCalendar_Message"],
                parseMode: ParseMode.Html,
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
        
        var calendar = await _calendarRepository.GetByUserIdAsync(user.Id);
        
        if (!Uri.TryCreate(request.Message.Text, UriKind.Absolute, out var uri))
        {
            await _botClient.SendMessage(
                chatId: request.Message.Chat.Id,
                text: _localizationProvider["MalformedLink_Message"],
                cancellationToken: cancellationToken);
            return null;
        }

        if (calendar != null)
        {
            calendar.CalendarUrl = uri.ToString();
            calendar.BitrixUserId = GetBitrixUserId(uri);
            
            await _calendarRepository.UpdateAsync(calendar);
        }
        else
        {
            var calendarUri = new Calendar(user.Id, uri.ToString(), _dateTimeProvider.UtcNow)
            {
                BitrixUserId = GetBitrixUserId(uri)
            };

            await _calendarRepository.CreateAsync(calendarUri);
        }
        
        var result = await _updateCalendarUseCase.Execute(user.Id, cancellationToken);

        await _botClient.SendMessage(
            chatId: request.Message.Chat.Id,
            text: _localizationProvider[result.message],
            cancellationToken: cancellationToken);
        
        if (result.isSuccessfull)
        {
            _userService.UpdateUserState(user.ChatId, UserState.MainMenu);
            return UserState.MainMenu;
        }
        return null;
    }
    
    private string? GetBitrixUserId(Uri uri)
    {
        var result = BitrixUserIdRegex().Match(uri.ToString());
        return result.Success ? result.Groups[1].Value : null;
    }
    
    [GeneratedRegex(@"user\/(\d{1,8})\/calendar")]
    private static partial Regex BitrixUserIdRegex();
}