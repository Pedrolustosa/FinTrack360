using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using FinTrack360.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Application.Features.Dashboard.UpcomingBills;

public class GetUpcomingBillsQueryHandler(IAppDbContext context) : IRequestHandler<GetUpcomingBillsQuery, List<UpcomingBillDto>>
{
    public async Task<List<UpcomingBillDto>> Handle(GetUpcomingBillsQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var upcomingPeriod = today.AddDays(30);

        var recurringTransactions = await context.RecurringTransactions
            .Where(rt => rt.UserId == request.UserId && (!rt.EndDate.HasValue || rt.EndDate.Value >= today))
            .ToListAsync(cancellationToken);

        var upcomingBills = new List<UpcomingBillDto>();

        foreach (var rt in recurringTransactions)
        {
            var nextDueDate = CalculateNextDueDate(rt, today);

            if (nextDueDate.HasValue && nextDueDate.Value <= upcomingPeriod)
            {
                upcomingBills.Add(new UpcomingBillDto
                {
                    Description = rt.Description,
                    Amount = rt.Amount,
                    DueDate = nextDueDate.Value
                });
            }
        }

        return upcomingBills.OrderBy(b => b.DueDate).Take(5).ToList();
    }

    private DateTime? CalculateNextDueDate(RecurringTransaction rt, DateTime today)
    {
        if (rt.StartDate > today) return rt.StartDate;

        DateTime nextDate = rt.StartDate;
        while (nextDate < today)
        {
            nextDate = rt.Frequency switch
            {
                Frequency.Weekly => nextDate.AddDays(7),
                Frequency.BiWeekly => nextDate.AddDays(14),
                Frequency.Monthly => nextDate.AddMonths(1),
                Frequency.Yearly => nextDate.AddYears(1),
                _ => throw new ArgumentOutOfRangeException(nameof(rt.Frequency), "Unsupported frequency")
            };
        }

        if (rt.EndDate.HasValue && nextDate > rt.EndDate.Value) return null;

        return nextDate;
    }
}
