using FinTrack360.Domain.Enums;

namespace FinTrack360.Application.Features.Transactions.UpdateTransaction;

public record UpdateTransactionDto(
    string Description,
    decimal Amount,
    DateTime Date,
    Guid? CategoryId,
    TransactionType Type);
