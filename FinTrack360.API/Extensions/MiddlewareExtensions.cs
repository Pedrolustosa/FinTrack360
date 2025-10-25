using FinTrack360.API.Middleware;

namespace FinTrack360.API.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
