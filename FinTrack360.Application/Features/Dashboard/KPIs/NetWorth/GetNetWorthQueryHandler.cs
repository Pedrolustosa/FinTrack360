using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Application.Features.Dashboard.KPIs.NetWorth;

public class GetNetWorthQueryHandler(IAppDbContext context) : IRequestHandler<GetNetWorthQuery, NetWorthDto>
{
    public async Task<NetWorthDto> Handle(GetNetWorthQuery request, CancellationToken cancellationToken)
    {
        var userAccounts = await context.Accounts
            .Where(a => a.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        var userAssets = await context.Assets
            .Where(a => userAccounts.Select(ua => ua.Id).Contains(a.AccountId))
            .ToListAsync(cancellationToken);

        decimal totalPositiveAccounts = userAccounts
            .Where(a => a.Type == AccountType.Checking || a.Type == AccountType.Savings || a.Type == AccountType.Investment)
            .Sum(a => a.CurrentBalance);

        decimal totalInvestmentsValue = userAssets
            .Sum(a => a.Quantity * a.AverageCost);

        decimal totalLiabilities = userAccounts
            .Where(a => a.Type == AccountType.CreditCard)
            .Sum(a => a.CurrentBalance);

        decimal totalAssets = totalPositiveAccounts + totalInvestmentsValue;

        return new NetWorthDto
        {
            TotalAssets = totalAssets,
            TotalLiabilities = totalLiabilities,
            NetWorth = totalAssets - totalLiabilities
        };
    }
}
