using CalendarNotificationBot.Data.Repositories.Interfaces;
using CalendarNotificationBot.Domain.Models;
using CalendarNotificationBot.Domain.Models.Calendar;
using CalendarNotificationBot.Domain.Service.Interfaces;
using CalendarNotificationBot.Domain.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace CalendarNotificationBot.App.Controllers;

/// <summary>
/// Controller for work with calendar data.
/// </summary>
[ApiController]
[Route("api/calendar")]
public class CalendarController : ControllerBase
{
    /// <summary>
    /// Logger.
    /// </summary>
    private readonly ILogger<CalendarController> _logger;

    /// <summary>
    /// .ctor
    /// </summary>
    /// <param name="logger"></param>
    public CalendarController(ILogger<CalendarController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Update calendar for specified Bitrix user.
    /// </summary>
    /// <param name="bitrixUserId">Bitrix user identifier</param>
    /// <param name="updateUserCalendarsUseCase"><see cref="UpdateUserCalendarsUseCase"/></param>
    /// <param name="ct">Cancellation token</param>
    [HttpPatch("update/bitrixUser/{bitrixUserId}")]
    public async Task UpdatePersonCalendarAsync(
        [FromRoute] string bitrixUserId,
        [FromServices] UpdateUserCalendarsUseCase updateUserCalendarsUseCase,
        CancellationToken ct)
    {
        await updateUserCalendarsUseCase.Execute(new[] { bitrixUserId }, ct);
        
        _logger.LogInformation("Update message received for {BitrixUserId}", bitrixUserId);
    }

    /// <summary>
    /// Update calendar for the list of Bitrix user.
    /// </summary>
    /// <param name="bitrixUsers"><see cref="BitrixUsersModel"/></param>
    /// <param name="updateUserCalendarsUseCase"><see cref="UpdateUserCalendarsUseCase"/></param>
    /// <param name="ct">Cancellation token</param>
    [HttpPatch("update/forBitrixUsers")]
    public async Task UpdatePersonCalendarAsync(
        [FromBody] BitrixUsersModel bitrixUsers,
        [FromServices] UpdateUserCalendarsUseCase updateUserCalendarsUseCase,
        CancellationToken ct)
    {
        await updateUserCalendarsUseCase.Execute(bitrixUsers.BitrixUserIds, ct);
        
        _logger.LogInformation(
            "Update message received for users: {BitrixUserIds}",
            string.Join(", ", bitrixUsers.BitrixUserIds));
    }
    
    /// <summary>
    /// A list of events planned for this day.
    /// </summary>
    /// <param name="userGuid">User Identifier</param>
    /// <param name="calendarService"><see cref="ICalendarService"/></param>
    /// <returns></returns>
    [HttpGet("todayEvents/{userGuid:guid}")]
    public IEnumerable<CalendarEventLocal> GetPersonCalendarAsync(
        [FromRoute] Guid userGuid,
        [FromServices] ICalendarService calendarService)
    {
        return calendarService.GetTodayNotificationsForUser(userGuid);
    }
}