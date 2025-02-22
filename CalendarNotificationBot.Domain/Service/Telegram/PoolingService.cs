using CalendarNotificationBot.Domain.Service.Telegram.Abstract;
using Microsoft.Extensions.Logging;

namespace CalendarNotificationBot.Domain.Service.Telegram;

// Compose Polling and ReceiverService implementations
public class PollingService : PollingServiceBase<ReceiverService>
{
    public PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
        : base(serviceProvider, logger)
    {
    }
}