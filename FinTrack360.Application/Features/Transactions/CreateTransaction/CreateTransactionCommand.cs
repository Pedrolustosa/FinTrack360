using FinTrack360.Domain.Enums;
using MediatR;

namespace FinTrack360.Application.Features.Transactions.CreateTransaction;

public record CreateTransactionCommand(
    Guid AccountId,
    string Description,
    decimal Amount,
    DateTime Date,
    Guid? CategoryId,
    TransactionType Type) : IRequest<Guid>;
