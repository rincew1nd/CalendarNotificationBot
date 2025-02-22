using System.Data;
using Dapper;

namespace CalendarNotificationBot.Data.Migrations;

public class MasterDatabase
{
    private readonly IDbConnection _connection;
    
    public MasterDatabase(DapperContext context)
    {
        _connection = context.CreateMasterConnection();
    }
    
    public void CreateDatabase(string dbName)
    {
        var query = "SELECT 1 FROM pg_database WHERE datname = @name";
        var parameters = new DynamicParameters();
        parameters.Add("name", dbName);
        
        var records = _connection.Query(query, parameters);
        if (!records.Any())
        {
            _connection.Execute($"CREATE DATABASE {dbName}");
        }
    }
}