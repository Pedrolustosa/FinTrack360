using FinTrack360.Application.Features.Dashboard.SpendingByCategoryChart;
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

namespace FinTrack360.Tests.Unit.Features.Dashboard.SpendingByCategoryChart;

public class GetSpendingByCategoryChartQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly GetSpendingByCategoryChartQueryHandler _handler;

    public GetSpendingByCategoryChartQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        var today = DateTime.UtcNow;
        var currentMonth = today.Month;
        var currentYear = today.Year;

        var userAccountId = Guid.NewGuid();
        var groceriesId = Guid.NewGuid();
        var transportId = Guid.NewGuid();

        var accounts = new List<Account>
        {
            new Account { Id = userAccountId, UserId = "test-user-id" }
        }.AsQueryable();

        var categories = new List<Category>
        {
            new Category { Id = groceriesId, Name = "Groceries" },
            new Category { Id = transportId, Name = "Transport" }
        }.AsQueryable();

        var transactions = new List<Transaction>
        {
            // Current month expenses for user
            new Transaction { AccountId = userAccountId, CategoryId = groceriesId, Date = new DateTime(currentYear, currentMonth, 5), Type = TransactionType.Expense, Amount = 200 },
            new Transaction { AccountId = userAccountId, CategoryId = groceriesId, Date = new DateTime(currentYear, currentMonth, 15), Type = TransactionType.Expense, Amount = 100 },
            new Transaction { AccountId = userAccountId, CategoryId = transportId, Date = new DateTime(currentYear, currentMonth, 10), Type = TransactionType.Expense, Amount = 150 },
            // Ignored data
            new Transaction { AccountId = userAccountId, CategoryId = groceriesId, Date = new DateTime(currentYear, currentMonth, 1), Type = TransactionType.Income, Amount = 1000 },
            new Transaction { AccountId = userAccountId, CategoryId = transportId, Date = new DateTime(currentYear, currentMonth - 1, 1), Type = TransactionType.Expense, Amount = 50 },
            new Transaction { AccountId = Guid.NewGuid(), CategoryId = groceriesId, Date = new DateTime(currentYear, currentMonth, 1), Type = TransactionType.Expense, Amount = 99 }
        }.AsQueryable();

        var mockAccounts = new Mock<DbSet<Account>>().SetupAsQueryable(accounts);
        var mockCategories = new Mock<DbSet<Category>>().SetupAsQueryable(categories);
        var mockTransactions = new Mock<DbSet<Transaction>>().SetupAsQueryable(transactions);

        _dbContextMock.Setup(db => db.Accounts).Returns(mockAccounts.Object);
        _dbContextMock.Setup(db => db.Categories).Returns(mockCategories.Object);
        _dbContextMock.Setup(db => db.Transactions).Returns(mockTransactions.Object);

        _handler = new GetSpendingByCategoryChartQueryHandler(_dbContextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSpendingGroupedByCategory_AndOrderedByAmount()
    {
        // Arrange
        var query = new GetSpendingByCategoryChartQuery("test-user-id");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        // Total spent = 200 + 100 (Groceries) + 150 (Transport) = 450
        Assert.Equal(2, result.Count);

        // First item should be Groceries (300)
        Assert.Equal("Groceries", result[0].CategoryName);
        Assert.Equal(300, result[0].TotalAmount);
        Assert.Equal(66.67, result[0].Percentage); // (300 / 450) * 100

        // Second item should be Transport (150)
        Assert.Equal("Transport", result[1].CategoryName);
        Assert.Equal(150, result[1].TotalAmount);
        Assert.Equal(33.33, result[1].Percentage); // (150 / 450) * 100
    }
}
