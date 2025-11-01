using FinTrack360.Application.Features.RecurringTransactions.GetRecurringTransactions;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
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

namespace FinTrack360.Tests.Unit.Features.RecurringTransactions.GetRecurringTransactions;

public class GetRecurringTransactionsQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly GetRecurringTransactionsQueryHandler _handler;

    public GetRecurringTransactionsQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "test-user-id") }, "mock"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var transactions = new List<RecurringTransaction>
        {
            new RecurringTransaction { UserId = "test-user-id", Description = "Later", StartDate = DateTime.UtcNow.AddDays(10), Account = new Account(), Category = new Category() },
            new RecurringTransaction { UserId = "test-user-id", Description = "Earlier", StartDate = DateTime.UtcNow.AddDays(5), Account = new Account(), Category = new Category() },
            new RecurringTransaction { UserId = "other-user-id", Description = "Other User", StartDate = DateTime.UtcNow, Account = new Account(), Category = new Category() }
        }.AsQueryable();

        var mockTransactions = new Mock<DbSet<RecurringTransaction>>().SetupAsQueryable(transactions);
        _dbContextMock.Setup(db => db.RecurringTransactions).Returns(mockTransactions.Object);

        _handler = new GetRecurringTransactionsQueryHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnUserTransactions_OrderedByStartDate()
    {
        // Arrange
        var query = new GetRecurringTransactionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal("Earlier", resultList[0].Description);
        Assert.Equal("Later", resultList[1].Description);
    }
}
