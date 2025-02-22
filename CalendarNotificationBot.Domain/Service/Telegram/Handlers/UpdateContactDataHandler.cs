using CalendarNotificationBot.Data.Repositories.Interfaces;
using CalendarNotificationBot.Domain.Resources;
using CalendarNotificationBot.Domain.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CalendarNotificationBot.Domain.Service.Telegram.Handlers;

/// <summary>
/// Update contact data command.
/// </summary>
/// <param name="Message"></param>
public record UpdateContactDataCommand(Message Message) : IRequest<UserState?>;

/// <summary>
/// Update contact data.
/// </summary>
public class UpdateContactDataHandler : IRequestHandler<UpdateContactDataCommand, UserState?>
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
    public UpdateContactDataHandler(
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
    public async Task<UserState?> Handle(UpdateContactDataCommand request, CancellationToken cancellationToken)
    {
        var result = false;

        var user = await _userRepository.GetByChatIdAsync(request.Message.Chat.Id);
        
        if (request.Message.From != null || request.Message.Contact != null)
        {
            await _userRepository.UpdateAsync(
                user.Id,
                user.ChatId,
                request.Message.Chat?.Username,
                request.Message.Contact?.FirstName ?? request.Message.Chat?.FirstName,
                request.Message.Contact?.LastName ?? request.Message.Chat?.LastName);
            result = true;
        }

        await _botClient.SendMessage(
            chatId: request.Message.Chat!.Id,
            text: result
                ? _localizationProvider["DataUpdated_Message"] : _localizationProvider["DataNotUpdated_Message"],
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);

        _userService.UpdateUserState(request.Message.Chat.Id, UserState.MainMenu);

        return UserState.MainMenu;
    }
}