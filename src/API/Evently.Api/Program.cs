using Evently.Api.Extensions;
using Evently.Modules.Events.Infrastructure;
using Evently.Common.Application;
using Evently.Common.Infrastructure;
using Serilog;
using Evently.Api.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Evently.Common.Presentation.Endpoints;
using Evently.Modules.Users.Infrastructure;
using Evently.Modules.Ticketing.Infrastructure;
using Evently.Modules.Attendance.Infrastructure;
using Evently.Api.OpenTelemetry;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(t => t.FullName?.Replace("+", "."));
});

builder.Services.AddApplication([
    Evently.Modules.Events.Application.AssemblyReference.Assembly,
    Evently.Modules.Users.Application.AssemblyReference.Assembly,
    Evently.Modules.Ticketing.Application.AssemblyReference.Assembly,
    Evently.Modules.Attendance.Application.AssemblyReference.Assembly]);

var redisConnectionString = builder.Configuration.GetConnectionString("Cache")!;
var databaseConnectionString = builder.Configuration.GetConnectionString("Database")!;
var mongoConnectionString = builder.Configuration.GetConnectionString("Mongo")!;

builder.Services.AddInfrastructure(
    DiagnosticsConfig.ServiceName,
    [TicketingModule.ConfigureConsumers],
    databaseConnectionString,
    redisConnectionString,
    mongoConnectionString);

builder.Configuration.AddModuleConfiguration(["events", "users", "ticketing", "attendance"]);

builder.Services.AddHealthChecks()
    .AddNpgSql(databaseConnectionString)
    .AddRedis(redisConnectionString)
    .AddMongoDb(mongoConnectionString)
    .AddUrlGroup(new Uri(builder.Configuration.GetValue<string>("KeyCloak:HealthUrl")!), HttpMethod.Get, "keycloak");

builder.Services.AddEventsModule(builder.Configuration);
builder.Services.AddUsersModule(builder.Configuration);
builder.Services.AddTicketingModule(builder.Configuration);
builder.Services.AddAttendanceModule(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

app.MapEndpoints();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseLogContextTraceLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.Run();
