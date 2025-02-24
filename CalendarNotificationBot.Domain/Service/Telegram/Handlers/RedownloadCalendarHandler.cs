using CalendarNotificationBot.Data.Repositories.Interfaces;
using CalendarNotificationBot.Domain.Resources;
using CalendarNotificationBot.Domain.Service.Interfaces;
using CalendarNotificationBot.Domain.UseCases;
using MediatR;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CalendarNotificationBot.Domain.Service.Telegram.Handlers;

/// <summary>
/// Command to redownload user's calendar.
/// </summary>
/// <param name="Message">Incoming message</param>
public record RedownloadCalendarCommand(Message Message) : IRequest<UserState?>;

/// <summary>
/// Redownload user's calendar.
/// </summary>
public class RedownloadCalendarHandler : IRequestHandler<RedownloadCalendarCommand, UserState?>
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
    /// Usecase for updating user's calendar.
    /// </summary>
    private readonly UpdateCalendarUseCase _updateCalendarUseCase;

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
    public RedownloadCalendarHandler(
        ITelegramBotClient botClient,
        IUserRepository userRepository,
        UpdateCalendarUseCase updateCalendarUseCase,
        IUserService userService,
        IStringLocalizer<SharedResource> localizationProvider)
    {
        _botClient = botClient;
        _userRepository = userRepository;
        _updateCalendarUseCase = updateCalendarUseCase;
        _userService = userService;
        _localizationProvider = localizationProvider;
    }

    /// <summary>
    /// Handle.
    /// </summary>
    public async Task<UserState?> Handle(RedownloadCalendarCommand request, CancellationToken cancellationToken)
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

        var result = await _updateCalendarUseCase.Execute(user.Id, cancellationToken);
        
        await _botClient.SendMessage(
            chatId: request.Message.Chat.Id,
            text: _localizationProvider[result.message],
            cancellationToken: cancellationToken);
        
        _userService.UpdateUserState(user.ChatId, UserState.MainMenu);
        return null;
    }
}