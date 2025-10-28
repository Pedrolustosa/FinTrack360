using MediatR;

namespace FinTrack360.Application.Features.Dashboard.AccountSummary;

public record GetAccountSummaryQuery(string UserId) : IRequest<List<AccountSummaryDto>>;
