using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.Transactions.GetTransactionById;

public class GetTransactionByIdQueryHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetTransactionByIdQuery, Transaction?>
{
    public async Task<Transaction?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        var account = await dbContext.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.AccountId && a.UserId == userId, cancellationToken);

        if (account == null) return null; // Or throw an exception

        return await dbContext.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.AccountId == request.AccountId, cancellationToken);
    }
}
