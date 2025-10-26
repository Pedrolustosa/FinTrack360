using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using FinTrack360.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.FinancialGoals.ContributeToFinancialGoal;

public class ContributeToFinancialGoalCommandHandler(IAppDbContext dbContext, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<ContributeToFinancialGoalCommand, Unit>
{
    public async Task<Unit> Handle(ContributeToFinancialGoalCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not found.");

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var goal = await dbContext.FinancialGoals.FirstOrDefaultAsync(fg => fg.Id == request.GoalId && fg.UserId == userId, cancellationToken) ?? throw new Exception("Financial goal not found.");
            var fromAccount = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.FromAccountId && a.UserId == userId, cancellationToken) ?? throw new Exception("Source account not found.");
            var toAccount = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.ToAccountId && a.UserId == userId, cancellationToken) ?? throw new Exception("Destination account not found.");

            if (fromAccount.CurrentBalance < request.Amount) throw new Exception("Insufficient funds in the source account.");

            // 1. Update goal's current amount
            goal.CurrentAmount += request.Amount;

            // 2. Update account balances
            fromAccount.CurrentBalance -= request.Amount;
            toAccount.CurrentBalance += request.Amount;

            // 3. Create debit transaction
            var debitTransaction = new Transaction
            {
                AccountId = fromAccount.Id,
                Amount = -request.Amount,
                Date = DateTime.UtcNow,
                Description = $"Contribution to goal: {goal.Name}",
                Type = TransactionType.Transfer
            };

            // 4. Create credit transaction
            var creditTransaction = new Transaction
            {
                AccountId = toAccount.Id,
                Amount = request.Amount,
                Date = DateTime.UtcNow,
                Description = $"Contribution from goal: {goal.Name}",
                Type = TransactionType.Transfer
            };

            dbContext.Transactions.Add(debitTransaction);
            dbContext.Transactions.Add(creditTransaction);

            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return Unit.Value;
        }
        catch (Exception)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
