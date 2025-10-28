using FinTrack360.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<ApplicationUser> Users { get; }
    DbSet<ActivityLog> ActivityLogs { get; }
    DbSet<Account> Accounts { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<Category> Categories { get; }
    DbSet<Budget> Budgets { get; }
    DbSet<Asset> Assets { get; }
    DbSet<RecurringTransaction> RecurringTransactions { get; }
    DbSet<FinancialGoal> FinancialGoals { get; }
    DbSet<Debt> Debts { get; }
    DbSet<CategorizationRule> CategorizationRules { get; }
    DbSet<Notification> Notifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
