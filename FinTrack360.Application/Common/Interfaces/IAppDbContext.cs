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

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
