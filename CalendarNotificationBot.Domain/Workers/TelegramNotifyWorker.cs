using System.Globalization;
using CalendarNotificationBot.Data.Repositories.Interfaces;
using CalendarNotificationBot.Domain.Extensions;
using CalendarNotificationBot.Domain.Models.Calendar;
using CalendarNotificationBot.Domain.Resources;
using CalendarNotificationBot.Domain.Service.Interfaces;
using CalendarNotificationBot.Domain.Workers.CronJob;
using CalendarNotificationBot.Infrastructure.DateTimeProvider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CalendarNotificationBot.Domain.Workers;

/// <summary>
/// Worker to notify user's about upcoming events. 
/// </summary>
public class TelegramNotifyWorker : CronWorkerService
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ICalendarService _calendarService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelegramNotifyWorker> _logger;

    public TelegramNotifyWorker(
        ITelegramBotClient telegramBotClient,
        ICalendarService calendarService,
        IServiceProvider serviceProvider,
        IDateTimeProvider dateTimeProvider,
        ILogger<TelegramNotifyWorker> logger,
        IScheduleConfig<TelegramNotifyWorker> options)
        : base(dateTimeProvider, options.CronExpression, logger)
    {
        _telegramBotClient = telegramBotClient;
        _calendarService = calendarService;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task DoWork(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching for upcoming event's");

        await using var scope = _serviceProvider.CreateAsyncScope();
        var userRepository = scope.ServiceProvider.GetService<IUserRepository>();
        var localizationProvider = scope.ServiceProvider.GetService<IStringLocalizer<SharedResource>>();

        var usersWithCalendar = await userRepository!.GetUsersWithCalendarAsync();

        foreach (var user in usersWithCalendar)
        {
            var checkDate = DateTimeProvider.UtcNow.AddMinutes(user.NotificationTime);
            
            var upcomingUsersEvents = _calendarService.GetUpcomingNotificationsForUser(user.Id, checkDate);
            
            foreach (var userEvent in upcomingUsersEvents ?? [])
            {
                CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = new CultureInfo(user.Culture);
                
                await _telegramBotClient.SendMessage(
                    user.ChatId,
                    string.Format(
                        localizationProvider!["CalendarEvent_Message"],
                        userEvent.Summary.EscapeStringForHtml(),
                        userEvent.Status.EscapeStringForHtml(),
                        userEvent.Description.EscapeStringForHtml(),
                        userEvent.Location.EscapeStringForHtml(),
                        userEvent.StartTime.ToUserTimeZone(user).ToString("dd-MM-yyyy HH:mm:ss"),
                        userEvent.Duration.TotalMinutes,
                        userEvent.EndTime.ToUserTimeZone(user).ToString("dd-MM-yyyy HH:mm:ss")
                    ),
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);
                
                userEvent.HasBeenSent = true;
            }
        }
    }
}