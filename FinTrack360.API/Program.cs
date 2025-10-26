using FinTrack360.API.Extensions;
using FinTrack360.API.Middleware;
using FinTrack360.Infrastructure.IoC;
using Hangfire;
using Hangfire.Storage.SQLite;
using FinTrack360.Application.Common.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerConfig();

// Hangfire Configuration
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSQLiteStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<TokenRevocationMiddleware>();
app.UseAuthorization();

app.MapControllers();

// Hangfire Dashboard and Job Scheduling
app.UseHangfireDashboard();
RecurringJob.AddOrUpdate<IRecurringTransactionService>(
    "process-recurring-transactions",
    service => service.ProcessRecurringTransactionsAsync(),
    Cron.Daily); // Runs every day at midnight

app.Run();