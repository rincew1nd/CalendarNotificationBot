using CalendarNotificationBot.Data.Repositories.Interfaces;
using CalendarNotificationBot.Domain.Service.Interfaces;
using CalendarNotificationBot.Infrastructure.DateTimeProvider;
using Microsoft.Extensions.Logging;

namespace CalendarNotificationBot.Domain.UseCases
{
    /// <summary>.
    /// Download calendar data for users and start update process.
    /// </summary>
    public class UpdateCalendarUseCase
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<UpdateCalendarUseCase> _logger;
        
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
        /// DateTime provider.
        /// </summary>
        private readonly IDateTimeProvider _dateTimeProvider;

        /// <summary>
        /// .ctor
        /// </summary>
        public UpdateCalendarUseCase(
            ILogger<UpdateCalendarUseCase> logger,
            ICalendarRepository calendarRepository,
            ICalendarService calendarService,
            IHttpClientFactory httpClientFactory,
            IDateTimeProvider dateTimeProvider)
        {
            _logger = logger;
            _calendarRepository = calendarRepository;
            _calendarService = calendarService;
            _httpClientFactory = httpClientFactory;
            _dateTimeProvider = dateTimeProvider;
        }
        
        /// <summary>
        /// Execute UseCase.
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="ct">Cancellation token</param>
        public async Task<(bool isSuccessfull, string message)> Execute(Guid userId, CancellationToken ct)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var userCalendar = await _calendarRepository.GetByUserIdAsync(userId);

            if (userCalendar == null)
            {
                _logger.LogError("Calendar \'{UserId}\' not found", userId);
                return (false, "CalendarNotFound_Message");
            }

            if ((_dateTimeProvider.Now - userCalendar.ModificationDate).Minutes < 5
                && _calendarService.CalendarExists(userId))
            {
                _logger.LogWarning("One calendar update per 5 minutes: {UserId}", userId);
                return (false, "CalendarUpdateTimeout_Message");
            }
            
            try
            {
                var response = await httpClient.GetAsync(userCalendar.CalendarUrl, ct);
                if (!response.IsSuccessStatusCode) return (true, "CalendarDownloadFailed_Message");
                
                var fileContent = await response.Content.ReadAsStringAsync(ct);
                var result = _calendarService.UpdateCalendars(
                    new Dictionary<Guid, string> { { userCalendar.UserId, fileContent } });

                if (result.TryGetValue(userCalendar.UserId, out var exception))
                {
                    throw exception;
                }

                await _calendarRepository.UpdateAsync(userCalendar);
                
                return (true, "CalendarUpdated_Message");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Exception during calendar update of user '{UserId}'",
                    userCalendar.UserId);
                return (false, "CalendarDownloadUnexpectedError_Message");
            }
        }
    }
}