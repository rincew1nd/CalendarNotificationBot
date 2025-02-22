using CalendarNotificationBot.Domain.Service.Telegram.Abstract;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace CalendarNotificationBot.Domain.Service.Telegram;

// Compose Receiver and UpdateHandler implementation
public class ReceiverService : ReceiverServiceBase<TelegramUpdateProcessor>
{
    public ReceiverService(
        ITelegramBotClient botClient,
        TelegramUpdateProcessor updateHandler,
        ILogger<ReceiverServiceBase<TelegramUpdateProcessor>> logger)
        : base(botClient, updateHandler, logger)
    {
    }
}