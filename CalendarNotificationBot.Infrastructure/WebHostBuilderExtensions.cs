using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CalendarNotificationBot.Infrastructure;

public static class WebHostBuilderExtensions
{
    /// <summary>
    /// Add serilog.
    /// </summary>
    public static IHostBuilder UseCustomSerilog(
        this IHostBuilder webHostBuilder,
        Action<HostBuilderContext, LoggerConfiguration>? configure = null)
    {
        webHostBuilder.UseSerilog(
            (hostingContext, loggerConfiguration) =>
            {
                var roService = hostingContext.Configuration.GetValue<string>("RoService");
                var hostname = hostingContext.Configuration.GetValue<string>("HOSTNAME");
                hostingContext.Configuration["Serilog:WriteTo:0:Args:typeName"] = null;
                loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.WithProperty("Service", roService)
                    .Enrich.WithProperty("Host", hostname);
                var action = configure;
                if (action == null)
                    return;
                action(hostingContext, loggerConfiguration);
            },
            writeToProviders: true);
        return webHostBuilder;
    }
}