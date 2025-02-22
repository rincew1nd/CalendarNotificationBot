using CalendarNotificationBot.Domain.Service;
using CalendarNotificationBot.Infrastructure.DateTimeProvider;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace CalendarNotificationBot.Test;

public class DuplicateCalendarEventsTest
{
    readonly Guid UserId = Guid.NewGuid();
    
    [Fact]
    public void CheckCalendarForDuplicates()
    {
        var loggerMock = new Mock<ILogger<CalendarService>>();
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(d => d.Today).Returns(new DateTime(2025, 02, 21));
        
        var calendarService = new CalendarService(loggerMock.Object, dateTimeProviderMock.Object);
        calendarService.UpdateCalendars(new Dictionary<Guid, string> { {UserId, DupedCalendar } });
        var events = calendarService.GetTodayNotificationsForUser(UserId);
        events.ShouldHaveSingleItem();
    }

    private const string DupedCalendar = """
                                         BEGIN:VCALENDAR
                                         PRODID:-//Bitrix//Bitrix Calendar//EN
                                         VERSION:2.0
                                         CALSCALE:GREGORIAN
                                         METHOD:PUBLISH
                                         X-WR-CALNAME:Test User
                                         X-WR-CALDESC:
                                         BEGIN:VEVENT
                                         DTSTART;VALUE=DATE-TIME:20250221T110000Z
                                         DTEND;VALUE=DATE-TIME:20250221T120000Z
                                         DTSTAMP:20250214T07010010800
                                         UID:486bd67af657d70eb7efd09fc23d1df3@bitrix
                                         SUMMARY:TestEvent
                                         DESCRIPTION:
                                         RRULE:FREQ=WEEKLY;INTERVAL=2;BYDAY=FR;UNTIL=20380102;WKST=MO

                                         LOCATION:Zoom
                                         SEQUENCE:0
                                         STATUS:CONFIRMED
                                         TRANSP:TRANSPARENT
                                         END:VEVENT

                                         BEGIN:VEVENT
                                         DTSTART;VALUE=DATE-TIME:20250221T110000Z
                                         DTEND;VALUE=DATE-TIME:20250221T120000Z
                                         DTSTAMP:20250214T07010010800
                                         UID:c9b11d297029938bbb8acb2eacf9b690@bitrix
                                         SUMMARY:TestEvent
                                         DESCRIPTION:

                                         LOCATION:Zoom
                                         SEQUENCE:0
                                         STATUS:CONFIRMED
                                         TRANSP:TRANSPARENT
                                         END:VEVENT
                                         END:VCALENDAR
                                         """;
}