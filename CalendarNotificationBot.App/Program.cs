using CalendarNotificationBot.App.HealthChecks;
using CalendarNotificationBot.Data;
using CalendarNotificationBot.Data.Migrations;
using CalendarNotificationBot.Domain;
using CalendarNotificationBot.Infrastructure;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseCustomSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();
builder.Services.AddHealthChecks().AddCheck<DbHealthCheck>("DB");

builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddDomainServices(builder.Configuration);
builder.Services.AddInfrastructureServices();

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 8192;
    logging.ResponseBodyLogLimit = 8192;
});

var app = builder.Build();

app.MigrateDatabase();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpLogging();

app.MapHealthChecks("/healthz");

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/debug-config", ctx => ctx.Response.WriteAsync(builder.Configuration.GetDebugView()));

app.Run();