using FinTrack360.Domain.Entities;
using MediatR;

namespace FinTrack360.Application.Features.RecurringTransactions.GetRecurringTransactionById;

public record GetRecurringTransactionByIdQuery(Guid Id) : IRequest<RecurringTransaction?>;
