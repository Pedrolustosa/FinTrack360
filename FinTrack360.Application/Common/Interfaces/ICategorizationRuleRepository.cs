using FinTrack360.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinTrack360.Application.Common.Interfaces;

public interface ICategorizationRuleRepository
{
    Task<CategorizationRule?> GetByIdAsync(Guid id);
    Task<IEnumerable<CategorizationRule>> GetByUserIdAsync(string userId);
    Task AddAsync(CategorizationRule rule);
    Task UpdateAsync(CategorizationRule rule);
    Task DeleteAsync(Guid id);
}
