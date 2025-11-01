using FinTrack360.Application.Features.DebtPayoffPlanner;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FinTrack360.Tests.Unit.Mocks;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Tests.Unit.Features.DebtPayoffPlanner;

public class GetSnowballPlanQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly GetSnowballPlanQueryHandler _handler;

    public GetSnowballPlanQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();

        var debts = new List<Debt>
        {
            new Debt { Id = Guid.NewGuid(), UserId = "test-user-id", Name = "Credit Card", CurrentBalance = 2000, InterestRate = 22.5m, MinimumPayment = 50 },
            new Debt { Id = Guid.NewGuid(), UserId = "test-user-id", Name = "Student Loan", CurrentBalance = 10000, InterestRate = 5.0m, MinimumPayment = 150 },
            new Debt { Id = Guid.NewGuid(), UserId = "test-user-id", Name = "Car Loan", CurrentBalance = 5000, InterestRate = 7.0m, MinimumPayment = 200 },
        }.AsQueryable();

        var mockDebts = new Mock<DbSet<Debt>>().SetupAsQueryable(debts);
        _dbContextMock.Setup(db => db.Debts).Returns(mockDebts.Object);

        _handler = new GetSnowballPlanQueryHandler(_dbContextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_GenerateCorrectSnowballPlan()
    {
        // Arrange
        var query = new GetSnowballPlanQuery("test-user-id", 100); // $100 extra payment

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal("Snowball", result.StrategyName);
        Assert.True(result.MonthsToPayoff > 0);
        Assert.True(result.TotalPaid > 17000);

        // Verify that the smallest debt ("Credit Card") is paid off first.
        var creditCardPayoffSteps = result.PayoffSteps.Where(s => s.DebtName == "Credit Card" && s.RemainingBalance <= 0).ToList();
        int? creditCardPayoffMonth = creditCardPayoffSteps.Any() ? creditCardPayoffSteps.Min(s => s.Month) : (int?)null;

        var otherDebtsPayoffMonths = result.PayoffSteps
            .Where(s => s.DebtName != "Credit Card" && s.RemainingBalance <= 0)
            .Select(s => s.Month)
            .ToList();

        Assert.True(creditCardPayoffMonth.HasValue, "Credit Card should have been paid off.");

        if (otherDebtsPayoffMonths.Any())
        {
            Assert.True(otherDebtsPayoffMonths.All(m => m >= creditCardPayoffMonth.Value), "No other debt should be paid off before the Credit Card.");
        }
    }
}
