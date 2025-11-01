using FinTrack360.Application.Features.Dashboard.RecentTransactions;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FinTrack360.Tests.Unit.Mocks;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Tests.Unit.Features.Dashboard.RecentTransactions;

public class GetRecentTransactionsQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly GetRecentTransactionsQueryHandler _handler;

    public GetRecentTransactionsQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();

        var userAccountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var accounts = new List<Account>
        {
            new Account { Id = userAccountId, UserId = "test-user-id", Name = "Checking" }
        }.AsQueryable();

        var transactions = new List<Transaction>
        {
            new Transaction { AccountId = userAccountId, Date = DateTime.UtcNow.AddDays(-1), Description = "Transaction 5", Amount = 50, CategoryId = categoryId, Category = new Category { Name = "Cat A" }, Account = accounts.First() },
            new Transaction { AccountId = userAccountId, Date = DateTime.UtcNow.AddDays(-2), Description = "Transaction 4", Amount = 40, Account = accounts.First() }, // Uncategorized
            new Transaction { AccountId = userAccountId, Date = DateTime.UtcNow.AddDays(-3), Description = "Transaction 3", Amount = 30, Account = accounts.First() },
            new Transaction { AccountId = userAccountId, Date = DateTime.UtcNow.AddDays(-4), Description = "Transaction 2", Amount = 20, Account = accounts.First() },
            new Transaction { AccountId = userAccountId, Date = DateTime.UtcNow.AddDays(-5), Description = "Transaction 1", Amount = 10, Account = accounts.First() },
            new Transaction { AccountId = userAccountId, Date = DateTime.UtcNow.AddDays(-6), Description = "Transaction 0 (should not be included)", Amount = 1, Account = accounts.First() },
        }.AsQueryable();

        var mockAccounts = new Mock<DbSet<Account>>().SetupAsQueryable(accounts);
        var mockTransactions = new Mock<DbSet<Transaction>>().SetupAsQueryable(transactions);

        _dbContextMock.Setup(db => db.Accounts).Returns(mockAccounts.Object);
        _dbContextMock.Setup(db => db.Transactions).Returns(mockTransactions.Object);

        _handler = new GetRecentTransactionsQueryHandler(_dbContextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnTop5RecentTransactions_OrderedByDate()
    {
        // Arrange
        var query = new GetRecentTransactionsQuery("test-user-id");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal("Transaction 5", result[0].Description);
        Assert.Equal("Transaction 1", result[4].Description);
        Assert.Equal("Cat A", result[0].CategoryName);
        Assert.Equal("Uncategorized", result[1].CategoryName);
        Assert.Equal("Checking", result[0].AccountName);
    }
}
