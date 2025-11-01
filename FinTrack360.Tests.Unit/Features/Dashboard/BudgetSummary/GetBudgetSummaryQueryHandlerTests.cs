using FinTrack360.Application.Features.Dashboard.BudgetSummary;
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

namespace FinTrack360.Tests.Unit.Features.Dashboard.BudgetSummary;

public class GetBudgetSummaryQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly GetBudgetSummaryQueryHandler _handler;

    public GetBudgetSummaryQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        var today = DateTime.UtcNow;
        var currentMonth = today.Month;
        var currentYear = today.Year;

        var category1Id = Guid.NewGuid();
        var category2Id = Guid.NewGuid();

        var budgets = new List<Budget>
        {
            new Budget { UserId = "test-user-id", Year = currentYear, Month = currentMonth, CategoryId = category1Id, Amount = 500, Category = new Category { Name = "Groceries" } },
            new Budget { UserId = "test-user-id", Year = currentYear, Month = currentMonth, CategoryId = category2Id, Amount = 100, Category = new Category { Name = "Entertainment" } },
            new Budget { UserId = "test-user-id", Year = currentYear - 1, Month = currentMonth, CategoryId = Guid.NewGuid(), Amount = 1000, Category = new Category { Name = "Old Budget" } }
        }.AsQueryable();

        var transactions = new List<Transaction>
        {
            // Matching expenses for budget 1 (Groceries) - under budget
            new Transaction { CategoryId = category1Id, Date = new DateTime(currentYear, currentMonth, 5), Type = TransactionType.Expense, Amount = 150 },
            new Transaction { CategoryId = category1Id, Date = new DateTime(currentYear, currentMonth, 15), Type = TransactionType.Expense, Amount = 200 },
            // Matching expenses for budget 2 (Entertainment) - over budget
            new Transaction { CategoryId = category2Id, Date = new DateTime(currentYear, currentMonth, 10), Type = TransactionType.Expense, Amount = 120 },
            // Non-matching data
            new Transaction { CategoryId = category1Id, Date = new DateTime(currentYear, currentMonth, 1), Type = TransactionType.Income, Amount = 1000 }, // Income, should be ignored
            new Transaction { CategoryId = category1Id, Date = new DateTime(currentYear, currentMonth - 1, 1), Type = TransactionType.Expense, Amount = 50 }, // Wrong month
            new Transaction { CategoryId = Guid.NewGuid(), Date = new DateTime(currentYear, currentMonth, 1), Type = TransactionType.Expense, Amount = 99 } // Wrong category
        }.AsQueryable();

        var mockBudgets = new Mock<DbSet<Budget>>().SetupAsQueryable(budgets);
        var mockTransactions = new Mock<DbSet<Transaction>>().SetupAsQueryable(transactions);

        _dbContextMock.Setup(db => db.Budgets).Returns(mockBudgets.Object);
        _dbContextMock.Setup(db => db.Transactions).Returns(mockTransactions.Object);

        _handler = new GetBudgetSummaryQueryHandler(_dbContextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnCorrectBudgetSummaries()
    {
        // Arrange
        var query = new GetBudgetSummaryQuery("test-user-id");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);

        var groceriesSummary = result.FirstOrDefault(r => r.CategoryName == "Groceries");
        Assert.NotNull(groceriesSummary);
        Assert.Equal(500, groceriesSummary.BudgetAmount);
        Assert.Equal(350, groceriesSummary.SpentAmount);
        Assert.Equal(150, groceriesSummary.RemainingAmount);
        Assert.False(groceriesSummary.IsOverBudget);

        var entertainmentSummary = result.FirstOrDefault(r => r.CategoryName == "Entertainment");
        Assert.NotNull(entertainmentSummary);
        Assert.Equal(100, entertainmentSummary.BudgetAmount);
        Assert.Equal(120, entertainmentSummary.SpentAmount);
        Assert.Equal(-20, entertainmentSummary.RemainingAmount);
        Assert.True(entertainmentSummary.IsOverBudget);
    }
}
