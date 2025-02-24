using System.Reflection;
using CalendarNotificationBot.Domain.Service;
using CalendarNotificationBot.Domain.Service.Interfaces;
using CalendarNotificationBot.Domain.Service.Telegram;
using CalendarNotificationBot.Domain.UseCases;
using CalendarNotificationBot.Domain.Workers;
using CalendarNotificationBot.Domain.Workers.CronJob;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace CalendarNotificationBot.Domain;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add domain services to DI.
    /// </summary>
    public static IServiceCollection AddDomainServices(
        this IServiceCollection services,
        IConfiguration configurationManager)
    {
        services.AddHttpClient();
        
        services.AddSingleton<ICalendarService, CalendarService>();
        services.AddSingleton<IUserService, UserService>();

        services.AddHttpClient("telegram_bot_client")
            .AddTypedClient<ITelegramBotClient>((httpClient, _) =>
            {
                var token = configurationManager.GetConfigString("Telegram:Token");
                TelegramBotClientOptions options = new(token);
                return new TelegramBotClient(options, httpClient);
            });

        services.AddCronJob<UpdateCalendarsWorker>(c =>
        {
            c.CronExpression = configurationManager.GetConfigString("Cron:UpdateCalendar");
        });
        
        services.AddCronJob<TelegramNotifyWorker>(c =>
        {
            c.CronExpression = configurationManager.GetConfigString("Cron:TelegramNotify");
        });
        
        services.AddCronJob<UpdateUpcomingEventsWorker>(c =>
        {
            c.CronExpression = configurationManager.GetConfigString("Cron:UpdateUpcomingEvents");
        });

        services.AddScoped<TelegramUpdateProcessor>();
        services.AddScoped<ReceiverService>();
        services.AddHostedService<PollingService>();

        services.AddTransient<UpdateBitrixCalendarsUseCase>();
        services.AddTransient<UpdateCalendarUseCase>();
        
        services.AddLocalization(options => options.ResourcesPath = "Resources");
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[] { "en-US", "ru-RU" };
            options.SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
        });

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        return services;
    }

    /// <summary>
    /// Get configuration string.
    /// </summary>
    private static string GetConfigString(this IConfiguration configurationManager, string token)
    {
        var value = configurationManager.GetValue<string>(token);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Configuration doesn't contains '{token}'");
        }

        return value;
    }
}