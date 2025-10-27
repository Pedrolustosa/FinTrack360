using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FinTrack360.Application.Features.Reports.GetSpendingByCategory;

namespace FinTrack360.Application.Features.Reports.GetSpendingByCategory;

public class GetSpendingByCategoryQueryHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetSpendingByCategoryQuery, IEnumerable<SpendingByCategoryDto>>
{
    public async Task<IEnumerable<SpendingByCategoryDto>> Handle(GetSpendingByCategoryQuery request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        var spendingReport = await dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.Account.UserId == userId &&
                        t.Type == TransactionType.Expense &&
                        t.Date >= request.StartDate &&
                        t.Date <= request.EndDate &&
                        t.Category != null)
            .GroupBy(t => t.Category!.Name)
            .Select(g => new SpendingByCategoryDto
            {
                CategoryName = g.Key,
                TotalAmount = g.Sum(t => t.Amount) * -1 // Invert the sign for display
            })
            .OrderByDescending(r => r.TotalAmount)
            .ToListAsync(cancellationToken);

        return spendingReport;
    }
}
