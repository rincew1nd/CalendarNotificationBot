using System.Data;
using CalendarNotificationBot.Data;
using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CalendarNotificationBot.App.HealthChecks;

public class DbHealthCheck : IHealthCheck
{
    private readonly IDbConnection _connection;
    private readonly IDbConnection _connectionMaster;

    public DbHealthCheck(DapperContext dbContext)
    {
        _connection = dbContext.CreateConnection();
        _connectionMaster = dbContext.CreateMasterConnection();
    }
    
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext hcContext,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _connection.Query<int>("select 1");
            _connectionMaster.Query<int>("select 1");
            return Task.FromResult(HealthCheckResult.Healthy("A healthy result."));
        }
        catch (Exception e)
        {
            return Task.FromResult(new HealthCheckResult(hcContext.Registration.FailureStatus, "An unhealthy result."));
        }
    }
}