using FinTrack360.Application.Features.Reports.GetSpendingByCategory;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using FinTrack360.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FinTrack360.Tests.Unit.Mocks;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Tests.Unit.Features.Reports.GetSpendingByCategory;

public class GetSpendingByCategoryQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly GetSpendingByCategoryQueryHandler _handler;

    public GetSpendingByCategoryQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "test-user-id") }, "mock"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var groceriesCat = new Category { Id = Guid.NewGuid(), Name = "Groceries" };
        var transportCat = new Category { Id = Guid.NewGuid(), Name = "Transport" };
        var userAccount = new Account { Id = Guid.NewGuid(), UserId = "test-user-id" };

        var transactions = new List<Transaction>
        {
            new Transaction { Account = userAccount, Category = groceriesCat, Type = TransactionType.Expense, Date = new DateTime(2025, 1, 5), Amount = -100 },
            new Transaction { Account = userAccount, Category = groceriesCat, Type = TransactionType.Expense, Date = new DateTime(2025, 1, 15), Amount = -50 },
            new Transaction { Account = userAccount, Category = transportCat, Type = TransactionType.Expense, Date = new DateTime(2025, 1, 10), Amount = -75 },
            new Transaction { Account = userAccount, Category = groceriesCat, Type = TransactionType.Income, Date = new DateTime(2025, 1, 20), Amount = 1000 }, // Should be ignored
            new Transaction { Account = userAccount, Category = transportCat, Type = TransactionType.Expense, Date = new DateTime(2024, 12, 31), Amount = -25 }, // Should be ignored
        }.AsQueryable();

        var mockTransactions = new Mock<DbSet<Transaction>>().SetupAsQueryable(transactions);
        _dbContextMock.Setup(db => db.Transactions).Returns(mockTransactions.Object);

        _handler = new GetSpendingByCategoryQueryHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSpendingByCategory_OrderedByTotalAmount()
    {
        // Arrange
        var query = new GetSpendingByCategoryQuery(new DateTime(2025, 1, 1), new DateTime(2025, 1, 31));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        
        // Groceries should be first (150)
        Assert.Equal("Groceries", resultList[0].CategoryName);
        Assert.Equal(150, resultList[0].TotalAmount);

        // Transport should be second (75)
        Assert.Equal("Transport", resultList[1].CategoryName);
        Assert.Equal(75, resultList[1].TotalAmount);
    }
}
