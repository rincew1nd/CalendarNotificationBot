using CalendarNotificationBot.Domain.UseCases;
using CalendarNotificationBot.Domain.Workers.CronJob;
using CalendarNotificationBot.Infrastructure.DateTimeProvider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CalendarNotificationBot.Domain.Workers;

/// <summary>
/// Worker for updating the user’s event calendar.
/// </summary>
public class UpdateCalendarsWorker : CronWorkerService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UpdateCalendarsWorker> _logger;

    public UpdateCalendarsWorker(
        IServiceProvider serviceProvider,
        IScheduleConfig<UpdateCalendarsWorker> config,
        ILogger<UpdateCalendarsWorker> logger)
        : base(
            serviceProvider.GetService<IDateTimeProvider>()!,
            config.CronExpression,
            logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task DoWork(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update of user calendars is initialized");

        await using var scope = _serviceProvider.CreateAsyncScope();

        var updateUserCalendarUseCase = scope.ServiceProvider.GetService<UpdateBitrixCalendarsUseCase>();
        await updateUserCalendarUseCase!.Execute(new CalendarUpdateCommand(), cancellationToken);
        
        _logger.LogInformation("Update of user calendars is completed");
    }
}