using Hangfire;
using FinTrack360.API.Extensions;
using FinTrack360.API.Middleware;
using FinTrack360.Infrastructure.IoC;
using FinTrack360.Application.Common.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfig();

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

app.UseHangfireDashboard("/jobs");

RecurringJob.AddOrUpdate<IRecurringTransactionService>(
    "process-recurring-transactions",
    service => service.ProcessRecurringTransactionsAsync(),
    Cron.Daily);

RecurringJob.AddOrUpdate<IDailyAlertJob>(
    "daily-alert-job",
    job => job.RunAsync(),
    Cron.Daily(3));

app.Run();