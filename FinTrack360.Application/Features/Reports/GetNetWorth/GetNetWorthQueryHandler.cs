using FinTrack360.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.Reports.GetNetWorth;

public class GetNetWorthQueryHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetNetWorthQuery, NetWorthDto>
{
    public async Task<NetWorthDto> Handle(GetNetWorthQuery request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        var totalAccountBalance = await dbContext.Accounts
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .SumAsync(a => a.CurrentBalance, cancellationToken);

        var totalAssetValue = await dbContext.Assets
            .AsNoTracking()
            .Where(a => a.Account.UserId == userId)
            .SumAsync(a => a.Quantity * a.AverageCost, cancellationToken);

        return new NetWorthDto
        {
            TotalAccountBalance = totalAccountBalance,
            TotalAssetValue = totalAssetValue
        };
    }
}
