using FinTrack360.Application.Features.RecurringTransactions.DeleteRecurringTransaction;
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

namespace FinTrack360.Tests.Unit.Features.RecurringTransactions.DeleteRecurringTransaction;

public class DeleteRecurringTransactionCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly DeleteRecurringTransactionCommandHandler _handler;
    private readonly Guid _transactionId;

    public DeleteRecurringTransactionCommandHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _transactionId = Guid.NewGuid();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "test-user-id") }, "mock"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var transactions = new List<RecurringTransaction>
        {
            new RecurringTransaction { Id = _transactionId, UserId = "test-user-id" }
        }.AsQueryable();

        var mockTransactions = new Mock<DbSet<RecurringTransaction>>().SetupAsQueryable(transactions);
        _dbContextMock.Setup(db => db.RecurringTransactions).Returns(mockTransactions.Object);

        _handler = new DeleteRecurringTransactionCommandHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_DeleteTransaction_WhenFound()
    {
        // Arrange
        var command = new DeleteRecurringTransactionCommand(_transactionId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContextMock.Verify(db => db.RecurringTransactions.Remove(It.Is<RecurringTransaction>(t => t.Id == _transactionId)), Times.Once);
        _dbContextMock.Verify(db => db.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenNotFound()
    {
        // Arrange
        var command = new DeleteRecurringTransactionCommand(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
