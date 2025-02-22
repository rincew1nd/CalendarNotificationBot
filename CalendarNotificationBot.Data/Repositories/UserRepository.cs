using System.Data;
using CalendarNotificationBot.Data.Entities;
using CalendarNotificationBot.Data.Repositories.Interfaces;
using Dapper;

namespace CalendarNotificationBot.Data.Repositories;

/// <summary>
/// User repository.
/// </summary>
public class UserRepository : IUserRepository
{
    /// <summary>
    /// Connection to DB.
    /// </summary>
    private readonly IDbConnection _connection;

    /// <summary>
    /// .ctor
    /// </summary>
    /// <param name="context"><see cref="DapperContext"/></param>
    public UserRepository(DapperContext context)
    {
        _connection = context.CreateConnection();
    }

    /// <inheritdoc/>
    public Task<IEnumerable<User>> GetAllAsync()
    {
        const string sql = @"SELECT * FROM Users";
        return _connection.QueryAsync<User>(sql);
    }

    /// <inheritdoc/>
    public Task<User?> GetByIdAsync(Guid id)
    {
        const string sql = @"SELECT * FROM Users WHERE ""Id"" = @Id";
        return _connection.QuerySingleOrDefaultAsync<User>(sql, new { id });
    }

    /// <inheritdoc/>
    public Task<User?> GetByChatIdAsync(long? chatId)
    {
        const string sql = @"SELECT * FROM Users WHERE ""ChatId"" = @ChatId";
        return _connection.QuerySingleOrDefaultAsync<User>(sql, new { chatId });
    }
    
    /// <inheritdoc/>
    public Task<IEnumerable<User>> GetUsersWithCalendarAsync()
    {
        const string sql = @"SELECT u.* from Users u join Calendars c on c.""UserId"" = u.""Id""";
        return _connection.QueryAsync<User>(sql);
    }

    /// <inheritdoc/>
    public async Task CreateAsync(User user)
    {
        const string sql = """
INSERT INTO Users ("Id", "ChatId", "UserName", "FirstName", "LastName")
VALUES (@Id, @ChatId, @UserName, @FirstName, @LastName)
""";
        await _connection.ExecuteAsync(sql, user);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Guid id, long chatId, string? userName, string? firstName, string? lastName)
    {
        const string sql = """
UPDATE Users
   SET "ChatId" = @ChatId, "UserName" = @UserName, "FirstName" = @FirstName, "LastName" = @LastName
 WHERE "Id" = @Id
""";
        await _connection.ExecuteAsync(sql, new { id, chatId, userName, firstName, lastName });
    }

    /// <inheritdoc/>
    public async Task UpdateTimeZoneAsync(Guid id, int timeZone)
    {
        const string sql = """UPDATE Users SET "TimeZone" = @timeZone WHERE "Id" = @Id""";
        await _connection.ExecuteAsync(sql, new { id, timeZone });
    }
    
    /// <inheritdoc/>
    public async Task UpdateCultureAsync(Guid id, string? culture)
    {
        const string sql = """UPDATE Users SET "Culture" = @culture WHERE "Id" = @Id""";
        await _connection.ExecuteAsync(sql, new { id, culture });
    }

    public async Task UpdateNotificationTime(Guid id, short notificationTime)
    {
        const string sql = """UPDATE Users SET "NotificationTime" = @notificationTime WHERE "Id" = @Id""";
        await _connection.ExecuteAsync(sql, new { id, notificationTime });
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id)
    {
        const string sql = @"DELETE FROM Users WHERE ""Id"" = @Id";
        await _connection.ExecuteAsync(sql, new { id });
    }
}
