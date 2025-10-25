using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.Profile.GetActivityLog;

public class GetActivityLogQueryHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetActivityLogQuery, IEnumerable<ActivityLog>>
{
    public async Task<IEnumerable<ActivityLog>> Handle(GetActivityLogQuery request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not found.");

        return await dbContext.ActivityLogs
            .Where(log => log.UserId == userId)
            .OrderByDescending(log => log.Timestamp)
            .ToListAsync(cancellationToken);
    }
}
