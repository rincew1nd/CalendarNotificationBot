# Bitrix Notification Bot

## [English 🇺🇸](README.md) | [Russian 🇷🇺](README.ru.md)

## Для чего приложение
Приложение предназначено для оповещения в Telegram о предстоящих событиях из загруженных .ICS календарей.

Через Telegram Bot'a можно зарегистрироваться и изменить настройки:
- Работа с календарём (пока только один на пользователя);
- Изменение часового пояса пользователя;
- Изменение времени оповещения.

В первую очередь приложение было написано для календарей из Bitrix, поэтому обновление календарей пользователей запускается не автоматически по расписанию, а через ручки:
- Обновление для нескольких пользователей ```api/calendar/update/forBitrixUsers```
- Обновление для конкретного пользователя ```api/calendar/update/forBitrixUser/{bitrixUserId}```

Так же есть возможность получить список предстоящих событий для пользователя по ручке:
```api/calendar/todayEvents/{userGuid:guid}```

Кастомизировать расписание задач можно через CRON выражения в конфигурации:
- UpdateCalendar: Обновление календаря пользователя (повторное выкачивание и кэширование календаря);
- TelegramNotify: Оповещение пользователей о предстоящем событии;
- UpdateUpcomingEvents: Выгрузка предстоящих событий на следующие 2 дня;

## Локальный запуск
- Открыть терминал в root папке;
- Создать сикреты для проекта;
> ```dotnet user-secrets init --project .\CalendarNotificationBot.App```
- Установить сикреты для Telegram API;
> ```dotnet user-[README.en.md](README.en.md)secrets set "Telegram:Token" "%TELEGRAM_TOKEN%" --project .\CalendarNotificationBot.App```
- Установить урлы для логов Serilog (_ОПЦИОНАЛЬНО_)
> ```dotnet user-secrets set "Serilog:WriteTo:0:Args:nodeUris" "%PATH_TO_ELASTIC%" --project .\CalendarNotificationBot.App```
- Запустить приложение
> ```dotnet run -[README.en.md](README.en.md)c Development --project CalendarNotificationBot.App```