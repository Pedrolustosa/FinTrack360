using FinTrack360.Application.Features.RecurringTransactions.GetRecurringTransactionById;
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

namespace FinTrack360.Tests.Unit.Features.RecurringTransactions.GetRecurringTransactionById;

public class GetRecurringTransactionByIdQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly GetRecurringTransactionByIdQueryHandler _handler;
    private readonly Guid _transactionId;

    public GetRecurringTransactionByIdQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _transactionId = Guid.NewGuid();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "test-user-id") }, "mock"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var transactions = new List<RecurringTransaction>
        {
            new RecurringTransaction { Id = _transactionId, UserId = "test-user-id", Account = new Account(), Category = new Category() },
            new RecurringTransaction { Id = Guid.NewGuid(), UserId = "other-user-id", Account = new Account(), Category = new Category() }
        }.AsQueryable();

        var mockTransactions = new Mock<DbSet<RecurringTransaction>>().SetupAsQueryable(transactions);
        _dbContextMock.Setup(db => db.RecurringTransactions).Returns(mockTransactions.Object);

        _handler = new GetRecurringTransactionByIdQueryHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnTransaction_WhenFoundForUser()
    {
        // Arrange
        var query = new GetRecurringTransactionByIdQuery(_transactionId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_transactionId, result.Id);
        Assert.NotNull(result.Account);
        Assert.NotNull(result.Category);
    }

    [Fact]
    public async Task Handle_Should_ReturnNull_WhenNotFound()
    {
        // Arrange
        var query = new GetRecurringTransactionByIdQuery(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
