using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.RecurringTransactions.GetRecurringTransactionById;

public class GetRecurringTransactionByIdQueryHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetRecurringTransactionByIdQuery, RecurringTransaction?>
{
    public async Task<RecurringTransaction?> Handle(GetRecurringTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        return await dbContext.RecurringTransactions
            .AsNoTracking()
            .Include(rt => rt.Account)
            .Include(rt => rt.Category)
            .FirstOrDefaultAsync(rt => rt.Id == request.Id && rt.UserId == userId, cancellationToken);
    }
}
