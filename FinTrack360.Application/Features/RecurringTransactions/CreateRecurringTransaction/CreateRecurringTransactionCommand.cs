using FinTrack360.Domain.Enums;
using MediatR;

namespace FinTrack360.Application.Features.RecurringTransactions.CreateRecurringTransaction;

public record CreateRecurringTransactionCommand(
    Guid AccountId,
    Guid CategoryId,
    string Description,
    decimal Amount,
    Frequency Frequency,
    DateTime StartDate,
    DateTime? EndDate) : IRequest<Guid>;
