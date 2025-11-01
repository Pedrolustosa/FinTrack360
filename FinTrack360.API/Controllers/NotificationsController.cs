using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinTrack360.Application.Features.Notifications.GetNotifications;
using FinTrack360.Application.Features.Notifications.MarkNotificationAsRead;

namespace FinTrack360.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationsController(ISender mediator) : ControllerBase
{
    private readonly ISender _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var notifications = await _mediator.Send(new GetNotificationsQuery());
        return Ok(notifications);
    }

    [HttpPost("{id:guid}/mark-as-read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        await _mediator.Send(new MarkNotificationAsReadCommand(id));
        return NoContent();
    }
}
