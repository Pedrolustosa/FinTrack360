using MediatR;

namespace FinTrack360.Application.Features.Transactions.DeleteTransaction;

public record DeleteTransactionCommand(Guid AccountId, Guid Id) : IRequest<Unit>;
