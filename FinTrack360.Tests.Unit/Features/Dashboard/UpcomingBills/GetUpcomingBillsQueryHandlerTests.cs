using FinTrack360.Application.Features.Dashboard.UpcomingBills;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using FinTrack360.Domain.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FinTrack360.Tests.Unit.Mocks;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Tests.Unit.Features.Dashboard.UpcomingBills;

public class GetUpcomingBillsQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly GetUpcomingBillsQueryHandler _handler;

    public GetUpcomingBillsQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        var today = DateTime.UtcNow.Date;

        var recurringTransactions = new List<RecurringTransaction>
        {
            // Should be included
            new RecurringTransaction { UserId = "test-user-id", Description = "Weekly Bill", Amount = 25, Frequency = Frequency.Weekly, StartDate = today.AddDays(-10) }, // Due in 4 days
            new RecurringTransaction { UserId = "test-user-id", Description = "Monthly Bill", Amount = 100, Frequency = Frequency.Monthly, StartDate = today.AddMonths(-1).AddDays(5) }, // Due in ~5 days
            new RecurringTransaction { UserId = "test-user-id", Description = "Bi-Weekly Bill", Amount = 50, Frequency = Frequency.BiWeekly, StartDate = today.AddDays(-12) }, // Due in 2 days
            new RecurringTransaction { UserId = "test-user-id", Description = "Yearly Bill", Amount = 1200, Frequency = Frequency.Yearly, StartDate = today.AddYears(-1).AddDays(15) }, // Due in 15 days
            new RecurringTransaction { UserId = "test-user-id", Description = "Future Start Bill", Amount = 200, Frequency = Frequency.Monthly, StartDate = today.AddDays(20) }, // Due in 20 days

            // Should be excluded
            new RecurringTransaction { UserId = "test-user-id", Description = "Expired Bill", Amount = 75, Frequency = Frequency.Monthly, StartDate = today.AddMonths(-2), EndDate = today.AddMonths(-1) },
            new RecurringTransaction { UserId = "test-user-id", Description = "Distant Future Bill", Amount = 300, Frequency = Frequency.Monthly, StartDate = today.AddDays(40) },
            new RecurringTransaction { UserId = "other-user-id", Description = "Other User Bill", Amount = 500, Frequency = Frequency.Monthly, StartDate = today.AddDays(1) },
        }.AsQueryable();

        var mockRecurringTransactions = new Mock<DbSet<RecurringTransaction>>().SetupAsQueryable(recurringTransactions);
        _dbContextMock.Setup(db => db.RecurringTransactions).Returns(mockRecurringTransactions.Object);

        _handler = new GetUpcomingBillsQueryHandler(_dbContextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnUpcomingBills_OrderedByDueDate()
    {
        // Arrange
        var query = new GetUpcomingBillsQuery("test-user-id");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal("Bi-Weekly Bill", result[0].Description);
        Assert.Equal("Weekly Bill", result[1].Description);
        Assert.Equal("Monthly Bill", result[2].Description);
        Assert.Equal("Yearly Bill", result[3].Description);
        Assert.Equal("Future Start Bill", result[4].Description);
    }
}
