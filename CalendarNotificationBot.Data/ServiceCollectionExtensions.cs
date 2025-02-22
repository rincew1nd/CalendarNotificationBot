using System.Reflection;
using CalendarNotificationBot.Data.Migrations;
using CalendarNotificationBot.Data.Repositories;
using CalendarNotificationBot.Data.Repositories.Interfaces;
using CalendarNotificationBot.Infrastructure.Database;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CalendarNotificationBot.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<DapperContext>();
        services.AddScoped<MasterDatabase>();
        
        services.AddLogging(c => c.AddFluentMigratorConsole())
            .AddFluentMigratorCore()
            .ConfigureRunner(c => c.AddPostgres11_0()
                .WithGlobalConnectionString(configuration.GetConnectionStringExtension("DefaultConnection"))
                .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations());
        
        services.AddScoped<ICalendarRepository, CalendarRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
}