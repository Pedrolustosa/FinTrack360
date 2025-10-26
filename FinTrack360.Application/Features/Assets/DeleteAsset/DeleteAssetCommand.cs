using MediatR;

namespace FinTrack360.Application.Features.Assets.DeleteAsset;

public record DeleteAssetCommand(Guid AccountId, Guid Id) : IRequest<Unit>;
