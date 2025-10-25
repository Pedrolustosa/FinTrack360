using System.Net;
using System.Text.Json;

namespace FinTrack360.API.Middleware;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        { 
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception has occurred.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var StatusCode = context.Response.StatusCode;
            var Message = "An internal server error has occurred.";
            var Detailed = ex.Message;

            var response = new { StatusCode, Message, Detailed };

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
