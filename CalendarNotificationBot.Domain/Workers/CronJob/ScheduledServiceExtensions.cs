using Microsoft.Extensions.DependencyInjection;

namespace CalendarNotificationBot.Domain.Workers.CronJob;

/// <summary>
/// Extension methods for <see cref="CronWorkerService"/>.
/// </summary>
public static class ScheduledServiceExtensions
{
    /// <summary>
    /// Add a new job with a cron schedule.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="options">Job schedule configuration</param>
    /// <typeparam name="T">Job type inherited from <see cref="CronWorkerService"/></typeparam>
    public static IServiceCollection AddCronJob<T>(this IServiceCollection services, Action<IScheduleConfig<T>> options)
        where T : CronWorkerService
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options), @"Schedule Configurations was not provided.");
        }

        var config = new ScheduleConfig<T>();
        options.Invoke(config);
        if (string.IsNullOrWhiteSpace(config.CronExpression))
        {
            throw new ArgumentNullException(nameof(options), "Empty Cron Expression is not allowed.");
        }

        services.AddSingleton<IScheduleConfig<T>>(config);
        services.AddHostedService<T>();
        return services;
    }
}