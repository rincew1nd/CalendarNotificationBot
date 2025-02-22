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
/// Command to register telegram user in application.
/// </summary>
/// <param name="Message">Incoming message</param>
public record RegisterUserCommand(Message Message) : IRequest<UserState?>;

/// <summary>
/// Register telegram user in application.
/// </summary>
public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, UserState?>
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
    public RegisterUserHandler(
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
    public async Task<UserState?> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByChatIdAsync(request.Message.Chat.Id);
        
        if (user == null)
        {
            await _botClient.SendMessage(
                chatId: request.Message.Chat.Id,
                text: _localizationProvider["Registration_Message"],
                cancellationToken: cancellationToken);
        
            user = new User()
            {
                Id = Guid.NewGuid(),
                ChatId = request.Message.Chat.Id,
                Username = request.Message.Chat.Username,
                Firstname = request.Message.Chat?.FirstName,
                Lastname = request.Message.Chat?.LastName,
                Culture = "en-US"
            };
            await _userRepository.CreateAsync(user);
        }
        
        _userService.UpdateUserState(user.ChatId, UserState.MainMenu);
        _userService.SetCultureInfo(user.ChatId, user.Culture);

        return UserState.MainMenu;
    }
}