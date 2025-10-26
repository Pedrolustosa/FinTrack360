using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.RecurringTransactions.CreateRecurringTransaction;

public class CreateRecurringTransactionCommandHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<CreateRecurringTransactionCommand, Guid>
{
    public async Task<Guid> Handle(CreateRecurringTransactionCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not found.");

        // Validate that the associated account and category belong to the user
        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.AccountId && a.UserId == userId, cancellationToken) ?? throw new Exception("Account not found or access denied.");
        var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.UserId == userId, cancellationToken) ?? throw new Exception("Category not found or access denied.");

        var recurringTransaction = new RecurringTransaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AccountId = request.AccountId,
            CategoryId = request.CategoryId,
            Description = request.Description,
            Amount = request.Amount,
            Frequency = request.Frequency,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        dbContext.RecurringTransactions.Add(recurringTransaction);
        await dbContext.SaveChangesAsync(cancellationToken);

        return recurringTransaction.Id;
    }
}
