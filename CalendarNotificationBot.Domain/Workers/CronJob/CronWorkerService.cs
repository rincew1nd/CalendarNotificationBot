using CalendarNotificationBot.Infrastructure.DateTimeProvider;
using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CalendarNotificationBot.Domain.Workers.CronJob;

/// <summary>
/// Base worker which starts a job by cron schedule.
/// </summary>
public abstract class CronWorkerService : BackgroundService
{
    /// <summary>
    /// Logger.
    /// </summary>
    private readonly ILogger<CronWorkerService> _logger;
    
    /// <summary>
    /// DateTime provider.
    /// </summary>
    protected readonly IDateTimeProvider DateTimeProvider;
    
    /// <summary>
    /// Cron expression.
    /// </summary>
    private readonly CronExpression _expression;

    /// <summary>
    /// .ctor
    /// </summary>
    protected CronWorkerService(IDateTimeProvider dateTimeProvider, string cronExpression, ILogger<CronWorkerService> logger)
    {
        DateTimeProvider = dateTimeProvider;
        _logger = logger;
        _expression = CronExpression.Parse(cronExpression, CronFormat.IncludeSeconds);
    }

    /// <summary>
    /// Schedule job execution.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var next = _expression.GetNextOccurrence(DateTimeProvider.UtcNow);
            
                if (next.HasValue)
                {
                    var delay = next.Value - DateTimeProvider.UtcNow;
                    if (delay.TotalMilliseconds > 0)
                    {
                        await Task.Delay(delay, cancellationToken);
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await DoWork(cancellationToken);
                        continue;
                    }
                }

                break;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unhandled exception in Cron task");
            }
        }
    }

    /// <summary>
    /// Logic to execute.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    protected abstract Task DoWork(CancellationToken cancellationToken);
}