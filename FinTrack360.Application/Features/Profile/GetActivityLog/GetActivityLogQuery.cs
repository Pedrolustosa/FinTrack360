using FinTrack360.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace FinTrack360.Application.Features.Profile.GetActivityLog;

public record GetActivityLogQuery : IRequest<IEnumerable<ActivityLog>>;
