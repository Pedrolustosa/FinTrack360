using FinTrack360.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace FinTrack360.Application.Features.RecurringTransactions.GetRecurringTransactions;

public record GetRecurringTransactionsQuery : IRequest<IEnumerable<RecurringTransaction>>;
