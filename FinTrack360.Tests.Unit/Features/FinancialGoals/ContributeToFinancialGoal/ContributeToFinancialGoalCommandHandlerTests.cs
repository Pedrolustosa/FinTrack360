using FinTrack360.Application.Features.FinancialGoals.ContributeToFinancialGoal;
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

namespace FinTrack360.Tests.Unit.Features.FinancialGoals.ContributeToFinancialGoal;

public class ContributeToFinancialGoalCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly ContributeToFinancialGoalCommandHandler _handler;
    private readonly Guid _goalId, _fromAccountId, _toAccountId;

    public ContributeToFinancialGoalCommandHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        _goalId = Guid.NewGuid();
        _fromAccountId = Guid.NewGuid();
        _toAccountId = Guid.NewGuid();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "test-user-id") }, "mock"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var goals = new List<FinancialGoal> { new FinancialGoal { Id = _goalId, UserId = "test-user-id", Name = "Vacation", CurrentAmount = 500 } }.AsQueryable();
        var accounts = new List<Account>
        {
            new Account { Id = _fromAccountId, UserId = "test-user-id", CurrentBalance = 1000 },
            new Account { Id = _toAccountId, UserId = "test-user-id", CurrentBalance = 2000 }
        }.AsQueryable();
        var transactions = new List<Transaction>().AsQueryable();

        var mockGoals = new Mock<DbSet<FinancialGoal>>().SetupAsQueryable(goals);
        var mockAccounts = new Mock<DbSet<Account>>().SetupAsQueryable(accounts);
        var mockTransactions = new Mock<DbSet<Transaction>>().SetupAsQueryable(transactions);

        _dbContextMock.Setup(db => db.FinancialGoals).Returns(mockGoals.Object);
        _dbContextMock.Setup(db => db.Accounts).Returns(mockAccounts.Object);
        _dbContextMock.Setup(db => db.Transactions).Returns(mockTransactions.Object);

        _handler = new ContributeToFinancialGoalCommandHandler(_dbContextMock.Object, _unitOfWorkMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_UpdateEntitiesAndCreateTransactions_WhenSuccessful()
    {
        // Arrange
        var command = new ContributeToFinancialGoalCommand(_goalId, _fromAccountId, _toAccountId, 250);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var goal = _dbContextMock.Object.FinancialGoals.First();
        var fromAccount = _dbContextMock.Object.Accounts.First(a => a.Id == _fromAccountId);
        var toAccount = _dbContextMock.Object.Accounts.First(a => a.Id == _toAccountId);

        Assert.Equal(750, goal.CurrentAmount);
        Assert.Equal(750, fromAccount.CurrentBalance);
        Assert.Equal(2250, toAccount.CurrentBalance);
        _dbContextMock.Verify(db => db.Transactions.Add(It.IsAny<Transaction>()), Times.Exactly(2));
        _unitOfWorkMock.Verify(uow => uow.CommitTransactionAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenInsufficientFunds()
    {
        // Arrange
        var command = new ContributeToFinancialGoalCommand(_goalId, _fromAccountId, _toAccountId, 1500); // More than balance

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        _unitOfWorkMock.Verify(uow => uow.RollbackTransactionAsync(CancellationToken.None), Times.Once);
    }
}
