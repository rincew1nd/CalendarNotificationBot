using CalendarNotificationBot.Domain.Service.Interfaces;
using CalendarNotificationBot.Domain.Workers.CronJob;
using CalendarNotificationBot.Infrastructure.DateTimeProvider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IServiceProvider = System.IServiceProvider;

namespace CalendarNotificationBot.Domain.Workers;

/// <summary>
/// Worker for updating upcoming user's events.
/// </summary>
public class UpdateUpcomingEventsWorker : CronWorkerService
{
    private readonly ICalendarService _calendarService;
    private readonly ILogger<UpdateUpcomingEventsWorker> _logger;

    public UpdateUpcomingEventsWorker(
        IServiceProvider serviceProvider,
        ICalendarService calendarService,
        IScheduleConfig<UpdateUpcomingEventsWorker> config,
        ILogger<UpdateUpcomingEventsWorker> logger)
        : base(
            serviceProvider.GetService<IDateTimeProvider>()!,
            config.CronExpression,
            logger)
    {
        _calendarService = calendarService;
        _logger = logger;
    }

    protected override Task DoWork(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initiated update of upcoming user events");

        _calendarService.UpdateUpcomingEvents();
        
        return Task.CompletedTask;
    }
}