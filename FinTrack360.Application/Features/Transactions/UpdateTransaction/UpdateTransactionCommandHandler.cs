using FinTrack360.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.Transactions.UpdateTransaction;

public class UpdateTransactionCommandHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<UpdateTransactionCommand, Unit>
{
    public async Task<Unit> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var account = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == request.AccountId && a.UserId == userId, cancellationToken) ?? throw new Exception("Account not found or access denied.");

        var transaction = await dbContext.Transactions
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.AccountId == request.AccountId, cancellationToken) ?? throw new Exception("Transaction not found.");

        // Revert the old transaction amount from the account balance
        account.CurrentBalance -= transaction.Amount;

        // Update transaction details
        transaction.Description = request.Description;
        transaction.Amount = request.Amount;
        transaction.Date = request.Date;
        transaction.CategoryId = request.CategoryId;
        transaction.Type = request.Type;

        // Apply the new transaction amount to the account balance
        account.CurrentBalance += transaction.Amount;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
