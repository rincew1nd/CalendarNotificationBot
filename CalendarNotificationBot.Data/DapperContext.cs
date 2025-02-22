using System.Data;
using System.Text.RegularExpressions;
using CalendarNotificationBot.Infrastructure.Database;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace CalendarNotificationBot.Data;

public class DapperContext
{
    /// <summary>
    /// Connection string for the database.
    /// </summary>
    private readonly string? _defaultConnection;
    
    /// <summary>
    /// Connection string for the master database.
    /// </summary>
    private readonly string? _masterConnection;
    
    /// <summary>
    /// .ctor
    /// </summary>
    public DapperContext(IConfiguration configuration)
    {
        _defaultConnection = configuration.GetConnectionStringExtension("DefaultConnection");
        _masterConnection = Regex.Replace(_defaultConnection, "Database=[^;]+;", "Database=postgres;");
    }

    /// <summary>
    /// Creates a connection string for the database.
    /// </summary>
    public IDbConnection CreateConnection() => new NpgsqlConnection(_defaultConnection);

    /// <summary>
    /// Creates a connection string for the master database.
    /// </summary>
    public IDbConnection CreateMasterConnection() => new NpgsqlConnection(_masterConnection);
}