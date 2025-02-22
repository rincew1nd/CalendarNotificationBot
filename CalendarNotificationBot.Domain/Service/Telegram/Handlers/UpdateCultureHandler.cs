using System.Globalization;
using CalendarNotificationBot.Data.Repositories.Interfaces;
using CalendarNotificationBot.Domain.Resources;
using CalendarNotificationBot.Domain.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CalendarNotificationBot.Domain.Service.Telegram.Handlers;

/// <summary>
/// Update contact data command.
/// </summary>
/// <param name="Message"></param>
public record UpdateCultureCommand(Message Message, string Callback) : IRequest<UserState?>;

/// <summary>
/// Update contact data.
/// </summary>
public class UpdateCultureHandler : IRequestHandler<UpdateCultureCommand, UserState?>
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
    public UpdateCultureHandler(
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
    public async Task<UserState?> Handle(UpdateCultureCommand request, CancellationToken cancellationToken)
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

        var culture = request.Callback.Split(";")[1];
        _userService.SetCultureInfo(request.Message.Chat.Id, culture);
        await _userRepository.UpdateCultureAsync(user.Id, culture);

        _userService.UpdateUserState(request.Message.Chat.Id, UserState.MainMenu);

        return UserState.MainMenu;
    }
}