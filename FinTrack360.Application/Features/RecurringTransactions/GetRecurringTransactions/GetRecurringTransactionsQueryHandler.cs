using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.RecurringTransactions.GetRecurringTransactions;

public class GetRecurringTransactionsQueryHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetRecurringTransactionsQuery, IEnumerable<RecurringTransaction>>
{
    public async Task<IEnumerable<RecurringTransaction>> Handle(GetRecurringTransactionsQuery request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        return await dbContext.RecurringTransactions
            .AsNoTracking()
            .Where(rt => rt.UserId == userId)
            .Include(rt => rt.Account)
            .Include(rt => rt.Category)
            .OrderBy(rt => rt.StartDate)
            .ToListAsync(cancellationToken);
    }
}
