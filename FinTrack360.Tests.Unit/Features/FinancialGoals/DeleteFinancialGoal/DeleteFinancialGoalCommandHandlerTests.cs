using FinTrack360.Application.Features.FinancialGoals.DeleteFinancialGoal;
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

namespace FinTrack360.Tests.Unit.Features.FinancialGoals.DeleteFinancialGoal;

public class DeleteFinancialGoalCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly DeleteFinancialGoalCommandHandler _handler;
    private readonly Guid _goalId;

    public DeleteFinancialGoalCommandHandlerTests()
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
            new FinancialGoal { Id = _goalId, UserId = "test-user-id" }
        }.AsQueryable();

        var mockGoals = new Mock<DbSet<FinancialGoal>>().SetupAsQueryable(goals);
        _dbContextMock.Setup(db => db.FinancialGoals).Returns(mockGoals.Object);

        _handler = new DeleteFinancialGoalCommandHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_DeleteGoal_WhenFound()
    {
        // Arrange
        var command = new DeleteFinancialGoalCommand(_goalId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContextMock.Verify(db => db.FinancialGoals.Remove(It.Is<FinancialGoal>(g => g.Id == _goalId)), Times.Once);
        _dbContextMock.Verify(db => db.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenNotFound()
    {
        // Arrange
        var command = new DeleteFinancialGoalCommand(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
