using FinTrack360.Application.Features.RecurringTransactions.UpdateRecurringTransaction;
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

namespace FinTrack360.Tests.Unit.Features.RecurringTransactions.UpdateRecurringTransaction;

public class UpdateRecurringTransactionCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly UpdateRecurringTransactionCommandHandler _handler;
    private readonly Guid _transactionId, _accountId, _categoryId;

    public UpdateRecurringTransactionCommandHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _transactionId = Guid.NewGuid();
        _accountId = Guid.NewGuid();
        _categoryId = Guid.NewGuid();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "test-user-id") }, "mock"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var accounts = new List<Account> { new Account { Id = _accountId, UserId = "test-user-id" } }.AsQueryable();
        var categories = new List<Category> { new Category { Id = _categoryId, UserId = "test-user-id" } }.AsQueryable();
        var transactions = new List<RecurringTransaction> { new RecurringTransaction { Id = _transactionId, UserId = "test-user-id" } }.AsQueryable();

        var mockAccounts = new Mock<DbSet<Account>>().SetupAsQueryable(accounts);
        var mockCategories = new Mock<DbSet<Category>>().SetupAsQueryable(categories);
        var mockTransactions = new Mock<DbSet<RecurringTransaction>>().SetupAsQueryable(transactions);

        _dbContextMock.Setup(db => db.Accounts).Returns(mockAccounts.Object);
        _dbContextMock.Setup(db => db.Categories).Returns(mockCategories.Object);
        _dbContextMock.Setup(db => db.RecurringTransactions).Returns(mockTransactions.Object);

        _handler = new UpdateRecurringTransactionCommandHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_UpdateTransaction_WhenValid()
    {
        // Arrange
        var newDate = DateTime.UtcNow.AddDays(1);
        var command = new UpdateRecurringTransactionCommand(_transactionId, _accountId, _categoryId, "Updated Desc", 99.99m, Frequency.Yearly, newDate, newDate.AddYears(1));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedTransaction = _dbContextMock.Object.RecurringTransactions.First();
        _dbContextMock.Verify(db => db.SaveChangesAsync(CancellationToken.None), Times.Once);
        Assert.Equal("Updated Desc", updatedTransaction.Description);
        Assert.Equal(99.99m, updatedTransaction.Amount);
        Assert.Equal(Frequency.Yearly, updatedTransaction.Frequency);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenTransactionNotFound()
    {
        // Arrange
        var command = new UpdateRecurringTransactionCommand(Guid.NewGuid(), _accountId, _categoryId, "Desc", 1m, Frequency.Weekly, DateTime.UtcNow, null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
