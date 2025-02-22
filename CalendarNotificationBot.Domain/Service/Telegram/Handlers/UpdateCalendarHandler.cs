﻿using System.Text.RegularExpressions;
using CalendarNotificationBot.Data.Repositories.Interfaces;
using CalendarNotificationBot.Domain.Resources;
using CalendarNotificationBot.Domain.Service.Interfaces;
using CalendarNotificationBot.Infrastructure.DateTimeProvider;
using MediatR;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CalendarNotificationBot.Domain.Service.Telegram.Handlers;

/// <summary>
/// Update calendar command.
/// </summary>
public record UpdateCalendarCommand(Message Message) : IRequest<UserState?>;

/// <summary>
/// Update calendar.
/// </summary>
public partial class UpdateCalendarHandler : IRequestHandler<UpdateCalendarCommand, UserState?>
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
    /// DateTime provider.
    /// </summary>
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>
    /// .ctor
    /// </summary>
    public UpdateCalendarHandler(
        ITelegramBotClient botClient,
        IUserRepository userRepository,
        ICalendarRepository calendarRepository,
        IUserService userService,
        IStringLocalizer<SharedResource> localizationProvider,
        IDateTimeProvider dateTimeProvider)
    {
        _botClient = botClient;
        _userRepository = userRepository;
        _calendarRepository = calendarRepository;
        _userService = userService;
        _localizationProvider = localizationProvider;
        _dateTimeProvider = dateTimeProvider;
    }
    
    /// <summary>
    /// Handle.
    /// </summary>
    public async Task<UserState?> Handle(UpdateCalendarCommand request, CancellationToken cancellationToken)
    {
        if (request.Message.ReplyMarkup != null)
        {
            await _botClient.SendMessage(
                chatId: request.Message.Chat.Id,
                text: _localizationProvider["UpdateCalendar_Message"],
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
        
        // TODO: Assert that file is compatible calendar.
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
        
        await _botClient.SendMessage(
            chatId: request.Message.Chat.Id,
            text:
            _localizationProvider["CalendarUpdated_Message"],
            cancellationToken: cancellationToken);
        
        _userService.UpdateUserState(user.ChatId, UserState.MainMenu);
        
        return UserState.MainMenu;
    }
    
    private string? GetBitrixUserId(Uri uri)
    {
        var result = BitrixUserIdRegex().Match(uri.ToString());
        return result.Success ? result.Groups[1].Value : null;
    }
    
    [GeneratedRegex(@"user\/(\d{1,8})\/calendar")]
    private static partial Regex BitrixUserIdRegex();
}