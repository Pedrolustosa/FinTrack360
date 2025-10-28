using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using FinTrack360.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinTrack360.Application.Services;

public class RuleEngineService : IRuleEngineService
{
    private readonly IAppDbContext _context;

    public RuleEngineService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Transaction>> ApplyCategorizationRulesAsync(string userId, IEnumerable<Transaction> transactions)
    {
        var rules = await _context.CategorizationRules
            .Where(r => r.UserId == userId)
            .ToListAsync();

        if (!rules.Any())
        {
            return transactions;
        }

        foreach (var transaction in transactions)
        {
            if (transaction.CategoryId.HasValue) continue;

            foreach (var rule in rules)
            {
                if (MatchesRule(transaction.Description, rule.RuleText, rule.RuleType))
                {
                    transaction.CategoryId = rule.CategoryId;
                    break; 
                }
            }
        }

        return transactions;
    }

    private bool MatchesRule(string description, string ruleText, RuleType ruleType)
    {
        return ruleType switch
        {
            RuleType.Contains => description.Contains(ruleText, System.StringComparison.OrdinalIgnoreCase),
            RuleType.StartsWith => description.StartsWith(ruleText, System.StringComparison.OrdinalIgnoreCase),
            RuleType.ExactMatch => description.Equals(ruleText, System.StringComparison.OrdinalIgnoreCase),
            _ => false,
        };
    }
}
