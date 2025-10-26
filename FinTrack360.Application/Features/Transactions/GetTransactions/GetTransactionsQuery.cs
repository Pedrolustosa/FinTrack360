using FinTrack360.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace FinTrack360.Application.Features.Transactions.GetTransactions;

public record GetTransactionsQuery(Guid AccountId) : IRequest<IEnumerable<Transaction>>;
