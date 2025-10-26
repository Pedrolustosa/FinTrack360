using FinTrack360.Domain.Enums;

namespace FinTrack360.Application.Features.Transactions.CreateTransaction;

public record CreateTransactionDto(
    string Description,
    decimal Amount,
    DateTime Date,
    Guid? CategoryId,
    TransactionType Type);
