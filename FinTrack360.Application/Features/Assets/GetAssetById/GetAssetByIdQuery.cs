using FinTrack360.Domain.Entities;
using MediatR;

namespace FinTrack360.Application.Features.Assets.GetAssetById;

public record GetAssetByIdQuery(Guid AccountId, Guid Id) : IRequest<Asset?>;
