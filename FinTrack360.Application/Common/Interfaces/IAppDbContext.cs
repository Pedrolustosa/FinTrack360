using FinTrack360.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<ApplicationUser> Users { get; }
    DbSet<ActivityLog> ActivityLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
