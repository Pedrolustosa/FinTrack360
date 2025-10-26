using MediatR;

namespace FinTrack360.Application.Features.RecurringTransactions.DeleteRecurringTransaction;

public record DeleteRecurringTransactionCommand(Guid Id) : IRequest<Unit>;
