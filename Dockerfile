FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . ./CalendarNotificationBot
WORKDIR /src/CalendarNotificationBot/CalendarNotificationBot.App
RUN dotnet restore "CalendarNotificationBot.App.csproj"
RUN dotnet build "CalendarNotificationBot.App.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CalendarNotificationBot.App.csproj" -c Release -o /app /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CalendarNotificationBot.App.dll"]