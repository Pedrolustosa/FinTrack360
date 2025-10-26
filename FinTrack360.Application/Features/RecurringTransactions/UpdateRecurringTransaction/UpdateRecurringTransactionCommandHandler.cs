using FinTrack360.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.RecurringTransactions.UpdateRecurringTransaction;

public class UpdateRecurringTransactionCommandHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<UpdateRecurringTransactionCommand, Unit>
{
    public async Task<Unit> Handle(UpdateRecurringTransactionCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var recurringTransaction = await dbContext.RecurringTransactions
            .FirstOrDefaultAsync(rt => rt.Id == request.Id && rt.UserId == userId, cancellationToken) ?? throw new Exception("Recurring transaction not found.");

        // Validate that the associated account and category belong to the user
        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.AccountId && a.UserId == userId, cancellationToken) ?? throw new Exception("Account not found or access denied.");
        var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.UserId == userId, cancellationToken) ?? throw new Exception("Category not found or access denied.");

        recurringTransaction.AccountId = request.AccountId;
        recurringTransaction.CategoryId = request.CategoryId;
        recurringTransaction.Description = request.Description;
        recurringTransaction.Amount = request.Amount;
        recurringTransaction.Frequency = request.Frequency;
        recurringTransaction.StartDate = request.StartDate;
        recurringTransaction.EndDate = request.EndDate;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
