using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using FinTrack360.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.Transactions.ImportTransactions;

public class ImportTransactionsCommandHandler(IAppDbContext dbContext, ITransactionParser transactionParser, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IRuleEngineService ruleEngineService)
    : IRequestHandler<ImportTransactionsCommand, Unit>
{
    public async Task<Unit> Handle(ImportTransactionsCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not found.");
        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.AccountId && a.UserId == userId, cancellationToken) ?? throw new Exception("Account not found or access denied.");

        await using var stream = request.File.OpenReadStream();
        var parsedTransactions = await transactionParser.ParseAsync(stream);

        var transactions = parsedTransactions.Select(parsedTx => new Transaction
        {
            AccountId = account.Id,
            Amount = parsedTx.Amount,
            Date = parsedTx.Date,
            Description = parsedTx.Description,
            Type = parsedTx.Amount > 0 ? TransactionType.Income : TransactionType.Expense,
        }).ToList();

        // Apply categorization rules
        var categorizedTransactions = await ruleEngineService.ApplyCategorizationRulesAsync(userId, transactions);

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            dbContext.Transactions.AddRange(categorizedTransactions);

            // Update account balance
            var totalImported = categorizedTransactions.Sum(t => t.Amount);
            account.CurrentBalance += totalImported;

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
