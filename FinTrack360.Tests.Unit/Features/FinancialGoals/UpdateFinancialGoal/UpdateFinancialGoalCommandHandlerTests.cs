using FinTrack360.Application.Features.FinancialGoals.UpdateFinancialGoal;
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

namespace FinTrack360.Tests.Unit.Features.FinancialGoals.UpdateFinancialGoal;

public class UpdateFinancialGoalCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly UpdateFinancialGoalCommandHandler _handler;
    private readonly Guid _goalId;

    public UpdateFinancialGoalCommandHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _goalId = Guid.NewGuid();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
        }, "mock"));

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var goals = new List<FinancialGoal>
        {
            new FinancialGoal { Id = _goalId, UserId = "test-user-id", Name = "Old Name", TargetAmount = 1000, TargetDate = DateTime.UtcNow.AddMonths(6) }
        }.AsQueryable();

        var mockGoals = new Mock<DbSet<FinancialGoal>>().SetupAsQueryable(goals);
        _dbContextMock.Setup(db => db.FinancialGoals).Returns(mockGoals.Object);

        _handler = new UpdateFinancialGoalCommandHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_UpdateGoal_WhenFound()
    {
        // Arrange
        var newTargetDate = DateTime.UtcNow.AddYears(1);
        var command = new UpdateFinancialGoalCommand(_goalId, "New Name", 2000, newTargetDate);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedGoal = _dbContextMock.Object.FinancialGoals.First();
        _dbContextMock.Verify(db => db.SaveChangesAsync(CancellationToken.None), Times.Once);
        Assert.Equal("New Name", updatedGoal.Name);
        Assert.Equal(2000, updatedGoal.TargetAmount);
        Assert.Equal(newTargetDate, updatedGoal.TargetDate);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenNotFound()
    {
        // Arrange
        var command = new UpdateFinancialGoalCommand(Guid.NewGuid(), "New Name", 2000, DateTime.UtcNow.AddYears(1));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
