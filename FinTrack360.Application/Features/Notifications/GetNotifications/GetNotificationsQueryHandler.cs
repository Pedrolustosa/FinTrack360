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

namespace FinTrack360.Application.Features.Notifications.GetNotifications;

public class GetNotificationsQueryHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetNotificationsQuery, IEnumerable<Notification>>
{
    public async Task<IEnumerable<Notification>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        return await dbContext.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.Timestamp)
            .ToListAsync(cancellationToken);
    }
}
