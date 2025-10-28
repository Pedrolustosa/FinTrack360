using FinTrack360.Domain.Enums;
using System;

namespace FinTrack360.Domain.Entities
{
    public class CategorizationRule
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public string RuleText { get; set; } = null!;
        public RuleType RuleType { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
