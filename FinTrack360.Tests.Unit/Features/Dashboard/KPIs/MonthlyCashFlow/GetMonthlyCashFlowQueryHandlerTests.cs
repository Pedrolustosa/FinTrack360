using FinTrack360.Application.Features.Dashboard.KPIs.MonthlyCashFlow;
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

namespace FinTrack360.Tests.Unit.Features.Dashboard.KPIs.MonthlyCashFlow;

public class GetMonthlyCashFlowQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly GetMonthlyCashFlowQueryHandler _handler;

    public GetMonthlyCashFlowQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        var today = DateTime.UtcNow;
        var currentMonth = today.Month;
        var currentYear = today.Year;

        var userAccountId = Guid.NewGuid();

        var accounts = new List<Account>
        {
            new Account { Id = userAccountId, UserId = "test-user-id" }
        }.AsQueryable();

        var transactions = new List<Transaction>
        {
            new Transaction { AccountId = userAccountId, Date = new DateTime(currentYear, currentMonth, 1), Type = TransactionType.Income, Amount = 2000 },
            new Transaction { AccountId = userAccountId, Date = new DateTime(currentYear, currentMonth, 5), Type = TransactionType.Expense, Amount = 100 },
            new Transaction { AccountId = userAccountId, Date = new DateTime(currentYear, currentMonth, 10), Type = TransactionType.Expense, Amount = 250 },
            new Transaction { AccountId = userAccountId, Date = new DateTime(currentYear, currentMonth - 1, 20), Type = TransactionType.Expense, Amount = 500 }, // Previous month
            new Transaction { AccountId = Guid.NewGuid(), Date = new DateTime(currentYear, currentMonth, 1), Type = TransactionType.Income, Amount = 5000 } // Other user
        }.AsQueryable();

        var mockAccounts = new Mock<DbSet<Account>>().SetupAsQueryable(accounts);
        var mockTransactions = new Mock<DbSet<Transaction>>().SetupAsQueryable(transactions);

        _dbContextMock.Setup(db => db.Accounts).Returns(mockAccounts.Object);
        _dbContextMock.Setup(db => db.Transactions).Returns(mockTransactions.Object);

        _handler = new GetMonthlyCashFlowQueryHandler(_dbContextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_CalculateMonthlyCashFlowCorrectly()
    {
        // Arrange
        var query = new GetMonthlyCashFlowQuery("test-user-id");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        // Total Income = 2000
        // Total Expenses = 100 + 250 = 350
        // Net Balance = 2000 - 350 = 1650
        Assert.Equal(2000, result.TotalIncome);
        Assert.Equal(350, result.TotalExpenses);
        Assert.Equal(1650, result.NetBalance);
    }
}
