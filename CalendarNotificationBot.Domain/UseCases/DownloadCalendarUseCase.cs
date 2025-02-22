using CalendarNotificationBot.Data.Repositories.Interfaces;
using CalendarNotificationBot.Domain.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace CalendarNotificationBot.Domain.UseCases
{
    /// <summary>.
    /// Download calendar data for users and start update process.
    /// </summary>
    public class UpdateUserCalendarsUseCase
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<UpdateUserCalendarsUseCase> _logger;
        
        /// <summary>
        /// Calendar repository.
        /// </summary>
        private readonly ICalendarRepository _calendarRepository;
        
        /// <summary>
        /// Calendar service.
        /// </summary>
        private readonly ICalendarService _calendarService;

        /// <summary>
        /// Http client factory.
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// .ctor
        /// </summary>
        public UpdateUserCalendarsUseCase(
            ILogger<UpdateUserCalendarsUseCase> logger,
            ICalendarRepository calendarRepository,
            ICalendarService calendarService,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _calendarRepository = calendarRepository;
            _calendarService = calendarService;
            _httpClientFactory = httpClientFactory;
        }
        
        /// <summary>
        /// Execute UseCase.
        /// </summary>
        /// <param name="bitrixUserIds">User's Bitrix identifiers</param>
        /// <param name="ct">Cancellation token</param>
        public async Task Execute(string[]? bitrixUserIds, CancellationToken ct)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var userCalendars = bitrixUserIds == null
                ? await _calendarRepository.GetAllAsync(true)
                : await _calendarRepository.GetByBitrixUserIdsAsync(bitrixUserIds);
            
            if (!userCalendars.Any()) return;
            
            Dictionary<Guid, string> updatedUserCalendars = new();
            
            await Parallel.ForEachAsync(
                userCalendars,
                new ParallelOptions { MaxDegreeOfParallelism = 3 },
                async (userCalendar, _) =>
                {
                    try
                    {
                        var response = await httpClient.GetAsync(userCalendar.CalendarUrl, ct);
                        if (response.IsSuccessStatusCode)
                        {
                            var fileContent = await response.Content.ReadAsStringAsync(ct);
                            updatedUserCalendars.Add(userCalendar.UserId, fileContent);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Exception during calendar update of user '{UserId}'",
                            userCalendar.UserId);
                    }
                });
            
            _calendarService.UpdateCalendars(updatedUserCalendars);
        }
    }
}