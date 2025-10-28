using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Application.Features.DebtPayoffPlanner;

public class GetSnowballPlanQueryHandler(IAppDbContext context) : IRequestHandler<GetSnowballPlanQuery, DebtPayoffPlanDto>
{
    public async Task<DebtPayoffPlanDto> Handle(GetSnowballPlanQuery request, CancellationToken cancellationToken)
    {
        var userDebts = await context.Debts
            .Where(d => d.UserId == request.UserId && d.CurrentBalance > 0)
            .OrderBy(d => d.CurrentBalance) // Snowball: Smallest balance first
            .ToListAsync(cancellationToken);

        return SimulatePayoff("Snowball", userDebts, request.ExtraPayment);
    }

    private DebtPayoffPlanDto SimulatePayoff(string strategyName, List<Debt> debts, decimal extraPayment)
    {
        var plan = new DebtPayoffPlanDto { StrategyName = strategyName };
        var debtSims = debts.Select(d => new DebtSimulation(d)).ToList();
        int month = 0;

        while (debtSims.Any(d => d.Balance > 0))
        {
            month++;
            decimal monthlyExtraPayment = extraPayment;
            decimal totalMinimumPayments = debtSims.Sum(d => d.IsPaidOff ? 0 : d.Debt.MinimumPayment);
            
            // Payments freed up from paid-off debts
            monthlyExtraPayment += debtSims.Where(d => d.IsPaidOff).Sum(d => d.Debt.MinimumPayment);

            // Pay minimums first
            foreach (var sim in debtSims.Where(s => !s.IsPaidOff))
            {
                decimal payment = sim.Debt.MinimumPayment;
                sim.Balance -= payment;
                plan.PayoffSteps.Add(new PayoffStepDto { Month = month, DebtId = sim.Debt.Id, DebtName = sim.Debt.Name, PaymentAmount = payment, RemainingBalance = sim.Balance });
                plan.TotalPaid += payment;
            }
            
            // Apply extra payment to the target debt
            var targetDebt = debtSims.FirstOrDefault(d => !d.IsPaidOff);
            if (targetDebt != null && monthlyExtraPayment > 0)
            {
                decimal payment = Math.Min(targetDebt.Balance, monthlyExtraPayment);
                targetDebt.Balance -= payment;
                plan.TotalPaid += payment;

                var existingStep = plan.PayoffSteps.First(s => s.Month == month && s.DebtId == targetDebt.Debt.Id);
                existingStep.PaymentAmount += payment;
                existingStep.RemainingBalance = targetDebt.Balance;
            }
            
            // Accrue interest for the next month
            foreach (var sim in debtSims.Where(s => !s.IsPaidOff))
            {
                sim.Balance += sim.Balance * (sim.Debt.InterestRate / 100 / 12);
            }
        }

        plan.MonthsToPayoff = month;
        return plan;
    }

    private class DebtSimulation
    {
        public Debt Debt { get; }
        public decimal Balance { get; set; }
        public bool IsPaidOff => Balance <= 0;

        public DebtSimulation(Debt debt)
        {
            Debt = debt;
            Balance = debt.CurrentBalance;
        }
    }
}
