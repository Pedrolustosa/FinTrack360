namespace FinTrack360.Application.Features.DebtPayoffPlanner;

public class DebtPayoffPlanDto
{
    public string StrategyName { get; set; } = string.Empty;
    public List<PayoffStepDto> PayoffSteps { get; set; } = [];
    public decimal TotalPaid { get; set; }
    public int MonthsToPayoff { get; set; }
}
