using MediatR;

namespace FinTrack360.Application.Features.Notifications.MarkNotificationAsRead;

public record MarkNotificationAsReadCommand(Guid Id) : IRequest<Unit>;
