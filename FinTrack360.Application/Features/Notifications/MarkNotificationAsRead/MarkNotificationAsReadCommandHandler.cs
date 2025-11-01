using FinTrack360.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.Notifications.MarkNotificationAsRead;

public class MarkNotificationAsReadCommandHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<MarkNotificationAsReadCommand, Unit>
{
    public async Task<Unit> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var notification = await dbContext.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.Id && n.UserId == userId, cancellationToken) ?? throw new Exception("Notification not found.");

        notification.IsRead = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
