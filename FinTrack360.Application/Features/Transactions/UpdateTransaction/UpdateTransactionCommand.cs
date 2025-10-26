using FinTrack360.Domain.Enums;
using MediatR;

namespace FinTrack360.Application.Features.Transactions.UpdateTransaction;

public record UpdateTransactionCommand(
    Guid AccountId,
    Guid Id,
    string Description,
    decimal Amount,
    DateTime Date,
    Guid? CategoryId,
    TransactionType Type) : IRequest<Unit>;
