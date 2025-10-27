using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.Reports.GetCashFlow;

public class GetCashFlowQueryHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetCashFlowQuery, IEnumerable<CashFlowDto>>
{
    public async Task<IEnumerable<CashFlowDto>> Handle(GetCashFlowQuery request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        var monthlyFlows = await dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.Account.UserId == userId && t.Date.Year == request.Year)
            .GroupBy(t => t.Date.Month)
            .Select(g => new
            {
                Month = g.Key,
                TotalIncome = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                TotalExpense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount) * -1
            })
            .ToListAsync(cancellationToken);

        // Ensure all 12 months are present, even if there are no transactions
        var result = Enumerable.Range(1, 12).Select(monthNum =>
        {
            var monthData = monthlyFlows.FirstOrDefault(mf => mf.Month == monthNum);
            return new CashFlowDto
            {
                Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNum),
                TotalIncome = monthData?.TotalIncome ?? 0,
                TotalExpense = monthData?.TotalExpense ?? 0
            };
        }).ToList();

        return result;
    }
}
