using System.Data;
using CalendarNotificationBot.Data.Entities;
using CalendarNotificationBot.Data.Repositories.Interfaces;
using CalendarNotificationBot.Infrastructure.DateTimeProvider;
using Dapper;

namespace CalendarNotificationBot.Data.Repositories;

/// <summary>
/// Calendar repository.
/// </summary>
public class CalendarRepository : ICalendarRepository
{
    /// <summary>
    /// DateTime provider.
    /// </summary>
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>
    /// Connection to DB.
    /// </summary>
    private readonly IDbConnection _connection;

    /// <summary>
    /// .ctor
    /// </summary>
    /// <param name="context"><see cref="DapperContext"/></param>
    /// <param name="dateTimeProvider"><see cref="IDateTimeProvider"/></param>
    public CalendarRepository(DapperContext context, IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        _connection = context.CreateConnection();
    }

    /// <inheritdoc/>
    public Task<IEnumerable<Calendar>> GetAllAsync(bool withoutBitrixUsers = false)
    {
        const string sql = @"SELECT * FROM Calendars";
        
        var query = sql;
        if (withoutBitrixUsers)
        {
            query = $@"{query} WHERE ""BitrixUserId"" IS NULL";
        }
        
        return _connection.QueryAsync<Calendar>(query);
    }

    /// <inheritdoc/>
    public Task<Calendar?> GetByUserIdAsync(Guid id)
    {
        const string sql = @"SELECT * FROM Calendars WHERE ""UserId"" = @Id";
        return _connection.QuerySingleOrDefaultAsync<Calendar>(sql, new { id });
    }

    /// <inheritdoc/>
    public Task<IEnumerable<Calendar>> GetByBitrixUserIdsAsync(string[] bitrixUserIds)
    {
        const string sql = @"SELECT * FROM Calendars WHERE ""BitrixUserId"" = ANY(@bitrixUserIds)";
        return _connection.QueryAsync<Calendar>(sql, new { bitrixUserIds });
    }

    /// <inheritdoc/>
    public async Task CreateAsync(Calendar calendar)
    {
        const string sql = """
INSERT INTO Calendars ("UserId", "CalendarUrl", "BitrixUserId")
VALUES (@UserId, @CalendarUrl, @BitrixUserId)
""";
        await _connection.ExecuteAsync(sql, calendar);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Calendar calendar)
    {
        calendar.ModificationDate = _dateTimeProvider.Now;
        
        const string sql = """
UPDATE Calendars
   SET "CalendarUrl" = @CalendarUrl, "ModificationDate" = @ModificationDate, "BitrixUserId" = @BitrixUserId
 WHERE "UserId" = @UserId
""";
        await _connection.ExecuteAsync(
            sql,
            new { calendar.UserId, calendar.CalendarUrl, ModificationDate = _dateTimeProvider.Now, calendar.BitrixUserId });
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id)
    {
        const string sql = @"DELETE FROM Calendars WHERE ""UserId"" = @Id";
        await _connection.ExecuteAsync(sql, new { id });
    }
}