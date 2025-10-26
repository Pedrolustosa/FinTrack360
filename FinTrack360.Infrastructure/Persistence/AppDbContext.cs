using System.Reflection;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FinTrack360.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<string>, string>(options), IAppDbContext
{
    public required DbSet<ActivityLog> ActivityLogs { get; set; }
    public required DbSet<Account> Accounts { get; set; }
    public required DbSet<Transaction> Transactions { get; set; }
    public required DbSet<Category> Categories { get; set; }
    public required DbSet<Budget> Budgets { get; set; }
    public required DbSet<Asset> Assets { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}