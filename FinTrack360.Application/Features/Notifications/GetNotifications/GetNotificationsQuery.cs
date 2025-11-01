using FinTrack360.Domain.Entities;
using MediatR;

namespace FinTrack360.Application.Features.Notifications.GetNotifications;

public record GetNotificationsQuery : IRequest<IEnumerable<Notification>>;
