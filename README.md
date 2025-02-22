# Calendar notification bot

## [English 🇺🇸](README.md) | [Russian 🇷🇺](README.ru.md)

# Purpose of the Application
The application is designed to send notifications via Telegram Bot about upcoming events from imported .ICS calendars.

Using the Telegram bot, users can register and modify settings:
- Manage their calendar (currently limited to one per user);
- Change their time zone;
- Change notification time.

Initially, it was developed for calendars from Bitrix, so user calendar updates are not scheduled automatically but are triggered via API endpoints:

- Update for multiple users: `api/calendar/update/forBitrixUsers`
- Update for a specific user: `api/calendar/update/forBitrixUser/{bitrixUserId}`

There is also an endpoint to retrieve a user's upcoming events:
`api/calendar/todayEvents/{userGuid:guid}`

You can customize the task schedule using CRON expressions in the configuration:
- UpdateCalendar: Updates the user's calendar (re-fetching and caching the calendar);
- TelegramNotify: Sends notifications to users about upcoming events;
- UpdateUpcomingEvents: Extracts upcoming events for the next two days.

## Local setup
- Open terminal in the project folder; 
- Initialize secrets for project
> ```dotnet user-secrets init --project .\CalendarNotificationBot.App```
- Setup token for telegram bot
> ```dotnet user-secrets set "Telegram:Token" "%TELEGRAM_TOKEN%" --project .\CalendarNotificationBot.App```
- Setup serilog node api (_OPTIONAL_)
> ```dotnet user-secrets set "Serilog:WriteTo:0:Args:nodeUris" "%PATH_TO_ELASTIC%" --project .\CalendarNotificationBot.App```
- Run application
> ```dotnet run -c Development --project CalendarNotificationBot.App```