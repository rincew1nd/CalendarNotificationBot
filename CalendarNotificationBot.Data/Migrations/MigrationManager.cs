using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CalendarNotificationBot.Data.Migrations;

public static class MigrationManager
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        
        var databaseService = scope.ServiceProvider.GetRequiredService<MasterDatabase>();
        var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        databaseService.CreateDatabase("calendar_notification_bot");
            
        migrationService.ListMigrations();
        migrationService.MigrateUp();

        return host;
    }
}