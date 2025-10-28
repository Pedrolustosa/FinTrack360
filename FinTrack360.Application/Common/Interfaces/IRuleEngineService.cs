using FinTrack360.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinTrack360.Application.Common.Interfaces;

public interface IRuleEngineService
{
    Task<IEnumerable<Transaction>> ApplyCategorizationRulesAsync(string userId, IEnumerable<Transaction> transactions);
}
