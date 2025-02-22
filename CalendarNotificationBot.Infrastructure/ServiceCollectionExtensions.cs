using CalendarNotificationBot.Infrastructure.DateTimeProvider;
using Microsoft.Extensions.DependencyInjection;

namespace CalendarNotificationBot.Infrastructure;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add infrastructure services to DI.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        return services;
    }
}