using FinTrack360.API.Extensions;
using FinTrack360.API.Middleware;
using FinTrack360.Infrastructure.IoC;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerConfig();
builder.Services.AddScoped<ITokenRevocationService, TokenRevocationService>();

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

app.Run();