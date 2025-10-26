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

namespace FinTrack360.Application.Features.Transactions.GetTransactions;

public class GetTransactionsQueryHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetTransactionsQuery, IEnumerable<Transaction>>
{
    public async Task<IEnumerable<Transaction>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        var accountExists = await dbContext.Accounts
            .AnyAsync(a => a.Id == request.AccountId && a.UserId == userId, cancellationToken);

        if (!accountExists) throw new Exception("Account not found or access denied.");

        return await dbContext.Transactions
            .Where(t => t.AccountId == request.AccountId)
            .OrderByDescending(t => t.Date)
            .ToListAsync(cancellationToken);
    }
}
