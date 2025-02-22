using Microsoft.Extensions.Configuration;

namespace CalendarNotificationBot.Infrastructure.Database;

public static class DatabaseExtension
{
    /// <summary>
    /// Get connection string by name.
    /// </summary>
    /// <param name="configuration">Application configuration</param>
    /// <param name="name">Connection string name</param>
    public static string GetConnectionStringExtension(this IConfiguration configuration, string name)
    {
        var connectionString = configuration.GetConnectionString(name);
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(name, $"Connection string '{name}' was not found");
        }

        return connectionString;
    }
}