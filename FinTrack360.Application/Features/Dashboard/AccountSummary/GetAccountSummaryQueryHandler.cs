using FinTrack360.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Application.Features.Dashboard.AccountSummary;

public class GetAccountSummaryQueryHandler(IAppDbContext context) : IRequestHandler<GetAccountSummaryQuery, List<AccountSummaryDto>>
{
    public async Task<List<AccountSummaryDto>> Handle(GetAccountSummaryQuery request, CancellationToken cancellationToken)
    {
        var accounts = await context.Accounts
            .Where(a => a.UserId == request.UserId)
            .OrderBy(a => a.Type) // Enum order is Checking, Savings, CreditCard, Investment - good enough
            .Select(a => new AccountSummaryDto
            {
                AccountId = a.Id,
                Name = a.Name,
                InstitutionName = a.InstitutionName,
                CurrentBalance = a.CurrentBalance,
                Type = a.Type
            })
            .ToListAsync(cancellationToken);

        return accounts;
    }
}
