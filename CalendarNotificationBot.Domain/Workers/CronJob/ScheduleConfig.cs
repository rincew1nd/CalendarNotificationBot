namespace CalendarNotificationBot.Domain.Workers.CronJob;

/// <summary>
/// Job schedule configuration.
/// </summary>
/// <typeparam name="T">Job type inherited from <see cref="CronWorkerService"/></typeparam>
public interface IScheduleConfig<T>
{
    /// <summary>
    /// Cron expression.
    /// </summary>
    string CronExpression { get; set; }
}

/// <inheritdoc/>
public class ScheduleConfig<T> : IScheduleConfig<T>
{
    /// <inheritdoc/>
    public string CronExpression { get; set; } = string.Empty;
}