using FinTrack360.Application.Features.Reports.GetCashFlow;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using FinTrack360.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FinTrack360.Tests.Unit.Mocks;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Tests.Unit.Features.Reports.GetCashFlow;

public class GetCashFlowQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly GetCashFlowQueryHandler _handler;

    public GetCashFlowQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "test-user-id") }, "mock"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var userAccount = new Account { Id = Guid.NewGuid(), UserId = "test-user-id" };

        var transactions = new List<Transaction>
        {
            // January 2025
            new Transaction { Account = userAccount, Date = new DateTime(2025, 1, 5), Type = TransactionType.Income, Amount = 2000 },
            new Transaction { Account = userAccount, Date = new DateTime(2025, 1, 15), Type = TransactionType.Expense, Amount = -500 },
            // March 2025
            new Transaction { Account = userAccount, Date = new DateTime(2025, 3, 10), Type = TransactionType.Expense, Amount = -150 },
            // Other year
            new Transaction { Account = userAccount, Date = new DateTime(2024, 1, 1), Type = TransactionType.Income, Amount = 1000 },
        }.AsQueryable();

        var mockTransactions = new Mock<DbSet<Transaction>>().SetupAsQueryable(transactions);
        _dbContextMock.Setup(db => db.Transactions).Returns(mockTransactions.Object);

        _handler = new GetCashFlowQueryHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnCashFlowForAllMonthsInYear()
    {
        // Arrange
        var query = new GetCashFlowQuery(2025);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(12, resultList.Count);

        // Check January
        var januaryData = resultList.First(r => r.Month == CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(1));
        Assert.Equal(2000, januaryData.TotalIncome);
        Assert.Equal(500, januaryData.TotalExpense);

        // Check February (no transactions)
        var februaryData = resultList.First(r => r.Month == CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(2));
        Assert.Equal(0, februaryData.TotalIncome);
        Assert.Equal(0, februaryData.TotalExpense);

        // Check March
        var marchData = resultList.First(r => r.Month == CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(3));
        Assert.Equal(0, marchData.TotalIncome);
        Assert.Equal(150, marchData.TotalExpense);
    }
}
