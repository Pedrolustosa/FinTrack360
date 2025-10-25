using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Common.Behaviors;

public class ActivityLoggingBehavior<TRequest, TResponse>(IHttpContextAccessor httpContextAccessor, IAppDbContext dbContext)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAuditableRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();

        var activityLog = new ActivityLog
        {
            UserId = request.UserId,
            ActionType = typeof(TRequest).Name,
            Timestamp = DateTime.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        dbContext.ActivityLogs.Add(activityLog);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await next(cancellationToken);
    }
}
