using CalendarNotificationBot.Domain.UseCases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CalendarNotificationBot.Domain.Workers;

/// <summary>
/// Job to initialize existed calendars after application initialization.
/// </summary>
public class InitializeCalendarsJob : IHostedService
{
    /// <summary>
    /// Service provider.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// .ctor
    /// </summary>
    public InitializeCalendarsJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Initialize calendars.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var updateUserCalendarUseCase = scope.ServiceProvider.GetService<UpdateBitrixCalendarsUseCase>();
        await updateUserCalendarUseCase!.Execute(new() { ForceUpdate = true }, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}