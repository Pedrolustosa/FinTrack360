using FinTrack360.Application.Features.RecurringTransactions.CreateRecurringTransaction;
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

namespace FinTrack360.Tests.Unit.Features.RecurringTransactions.CreateRecurringTransaction;

public class CreateRecurringTransactionCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly CreateRecurringTransactionCommandHandler _handler;
    private readonly Guid _accountId, _categoryId;

    public CreateRecurringTransactionCommandHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _accountId = Guid.NewGuid();
        _categoryId = Guid.NewGuid();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "test-user-id") }, "mock"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var accounts = new List<Account> { new Account { Id = _accountId, UserId = "test-user-id" } }.AsQueryable();
        var categories = new List<Category> { new Category { Id = _categoryId, UserId = "test-user-id" } }.AsQueryable();
        var recurringTransactions = new List<RecurringTransaction>().AsQueryable();

        var mockAccounts = new Mock<DbSet<Account>>().SetupAsQueryable(accounts);
        var mockCategories = new Mock<DbSet<Category>>().SetupAsQueryable(categories);
        var mockRecurringTransactions = new Mock<DbSet<RecurringTransaction>>().SetupAsQueryable(recurringTransactions);

        _dbContextMock.Setup(db => db.Accounts).Returns(mockAccounts.Object);
        _dbContextMock.Setup(db => db.Categories).Returns(mockCategories.Object);
        _dbContextMock.Setup(db => db.RecurringTransactions).Returns(mockRecurringTransactions.Object);

        _handler = new CreateRecurringTransactionCommandHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_CreateRecurringTransaction_WhenValid()
    {
        // Arrange
        var command = new CreateRecurringTransactionCommand(_accountId, _categoryId, "Netflix", 15.99m, Frequency.Monthly, DateTime.UtcNow, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContextMock.Verify(db => db.RecurringTransactions.Add(It.IsAny<RecurringTransaction>()), Times.Once);
        _dbContextMock.Verify(db => db.SaveChangesAsync(CancellationToken.None), Times.Once);
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenAccountNotFound()
    {
        // Arrange
        var command = new CreateRecurringTransactionCommand(Guid.NewGuid(), _categoryId, "Netflix", 15.99m, Frequency.Monthly, DateTime.UtcNow, null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenCategoryNotFound()
    {
        // Arrange
        var command = new CreateRecurringTransactionCommand(_accountId, Guid.NewGuid(), "Netflix", 15.99m, Frequency.Monthly, DateTime.UtcNow, null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
