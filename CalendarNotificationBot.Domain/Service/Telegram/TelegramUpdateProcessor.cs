using System.Globalization;
using CalendarNotificationBot.Domain.Service.Interfaces;
using CalendarNotificationBot.Domain.Service.Telegram.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using static System.Enum;

namespace CalendarNotificationBot.Domain.Service.Telegram;

public class TelegramUpdateProcessor : IUpdateHandler
{
    /// <summary>
    /// Telegram bot client.
    /// </summary>
    private readonly ITelegramBotClient _botClient;

    /// <summary>
    /// User service.
    /// </summary>
    private readonly IUserService _userService;
    
    /// <summary>
    /// Mediator CQRS.
    /// </summary>
    private readonly IMediator _mediator;
    
    /// <summary>
    /// Logger.
    /// </summary>
    private readonly ILogger<TelegramUpdateProcessor> _logger;

    /// <summary>
    /// .ctor
    /// </summary>
    public TelegramUpdateProcessor(
        ITelegramBotClient botClient,
        IUserService userService,
        IMediator mediator,
        ILogger<TelegramUpdateProcessor> logger)
    {
        _botClient = botClient;
        _userService = userService;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
        try
        {
            var handler = update switch
            {
                { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
                { EditedMessage: { } message } => BotOnMessageReceived(message, cancellationToken),
                { CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(callbackQuery, cancellationToken),
                _ => UnknownUpdateHandlerAsync(update, cancellationToken)
            };

            await handler;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error while processing request");
        }
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException =>
                $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", errorMessage);

        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    /// <summary>
    /// Process incoming message.
    /// </summary>
    private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        await ProcessUserMessage(message: message, cancellationToken: cancellationToken);
    }
    
    /// <summary>
    /// Process incoming callback query
    /// </summary>
    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        await _botClient.AnswerCallbackQuery(
            callbackQueryId: callbackQuery.Id,
            cancellationToken: cancellationToken);

        await ProcessUserMessage(callbackQuery: callbackQuery, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Process incoming user message.
    /// </summary>
    private async Task ProcessUserMessage(
        Message? message = null,
        CallbackQuery? callbackQuery = null,
        CancellationToken cancellationToken = default)
    {
        if (message == null && callbackQuery?.Message == null)
        {
            throw new ArgumentException("Message was not found");
        }

        UserState? userState = null;
        if (callbackQuery != null)
        {
            if (TryParse<UserState>(callbackQuery.Data?.Split(";")[0], out var userStateOut))
            {
                _userService.UpdateUserState(callbackQuery.Message!.Chat.Id, userStateOut);
            }
            message = callbackQuery.Message;
        }
        
        userState = _userService.GetUserState(message!.Chat.Id);
        
        while (userState != null)
        {
            var userCulture = _userService.GetCultureInfo(message!.Chat.Id);
            CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = new CultureInfo(userCulture);
            
            var action = userState switch
            {
                UserState.ShareContactData => _mediator.Send(new ShareContactDataCommand(message), cancellationToken),
                UserState.UpdateContactData => _mediator.Send(new UpdateContactDataCommand(message), cancellationToken),
                UserState.UpdateCalendar => _mediator.Send(new UpdateCalendarCommand(message), cancellationToken),
                UserState.ViewInfo => _mediator.Send(new ViewInfoCommand(message), cancellationToken),
                UserState.RemoveCalendar => _mediator.Send(new RemoveCalendarCommand(message), cancellationToken),
                UserState.UpdateTimeZone => _mediator.Send(new UpdateTimezoneCommand(message), cancellationToken),
                UserState.Register => _mediator.Send(new RegisterUserCommand(message), cancellationToken),
                UserState.UpdateCulture =>
                    _mediator.Send(new UpdateCultureCommand(message, callbackQuery!.Data!), cancellationToken),
                UserState.UpdateNotificationTime =>
                    _mediator.Send(new UpdateNotificationTimeCommand(message), cancellationToken),
                _ => _mediator.Send(new MainMenuCommand(message), cancellationToken)
            };
        
            userState = await action;
        }
    }

    private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}