using MediatR;

namespace FinTrack360.Application.Features.Dashboard.RecentTransactions;

public record GetRecentTransactionsQuery(string UserId) : IRequest<List<RecentTransactionDto>>;
