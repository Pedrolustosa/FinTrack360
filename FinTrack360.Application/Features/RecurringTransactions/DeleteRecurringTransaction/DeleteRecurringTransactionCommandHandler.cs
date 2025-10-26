using FinTrack360.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.RecurringTransactions.DeleteRecurringTransaction;

public class DeleteRecurringTransactionCommandHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<DeleteRecurringTransactionCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRecurringTransactionCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var recurringTransaction = await dbContext.RecurringTransactions
            .FirstOrDefaultAsync(rt => rt.Id == request.Id && rt.UserId == userId, cancellationToken) ?? throw new Exception("Recurring transaction not found.");

        dbContext.RecurringTransactions.Remove(recurringTransaction);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
