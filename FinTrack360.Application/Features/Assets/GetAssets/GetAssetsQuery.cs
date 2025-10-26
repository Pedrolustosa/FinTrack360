using FinTrack360.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace FinTrack360.Application.Features.Assets.GetAssets;

public record GetAssetsQuery(Guid AccountId) : IRequest<IEnumerable<Asset>>;
