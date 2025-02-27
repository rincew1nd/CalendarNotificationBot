using CalendarNotificationBot.Data.Repositories.Interfaces;
using CalendarNotificationBot.Domain.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace CalendarNotificationBot.Domain.UseCases
{
    /// <summary>.
    /// Download calendar data for users and start update process.
    /// </summary>
    public class UpdateBitrixCalendarsUseCase
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<UpdateBitrixCalendarsUseCase> _logger;
        
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
        public UpdateBitrixCalendarsUseCase(
            ILogger<UpdateBitrixCalendarsUseCase> logger,
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
        public async Task Execute(CalendarUpdateCommand command, CancellationToken ct)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var userCalendars = command switch
                {
                    { ForceUpdate: true } => await _calendarRepository.GetAllAsync(false),
                    { UserIds: not null } => await _calendarRepository.GetByUserIdsAsync(command.UserIds),
                    { BitrixUserIds: not null } => await _calendarRepository.GetByBitrixUserIdsAsync(
                        command.BitrixUserIds),
                    _ => await _calendarRepository.GetAllAsync(true)
                };

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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
    
    public class CalendarUpdateCommand
    {
        /// <summary>
        /// Force update for all available calendars.
        /// </summary>
        public bool ForceUpdate { get; set; } = false;

        /// <summary>
        /// Update calendar for specified users.
        /// </summary>
        public Guid[]? UserIds { get; set; } = null;
        
        /// <summary>
        /// Update calendar for specified bitrix users.
        /// </summary>
        public string[]? BitrixUserIds { get; set; } = null;
    }
}