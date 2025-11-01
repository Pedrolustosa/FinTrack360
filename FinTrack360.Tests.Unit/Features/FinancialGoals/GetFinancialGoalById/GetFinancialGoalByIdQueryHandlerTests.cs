using FinTrack360.Application.Features.FinancialGoals.GetFinancialGoalById;
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

namespace FinTrack360.Tests.Unit.Features.FinancialGoals.GetFinancialGoalById;

public class GetFinancialGoalByIdQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly GetFinancialGoalByIdQueryHandler _handler;
    private readonly Guid _goalId;

    public GetFinancialGoalByIdQueryHandlerTests()
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
            new FinancialGoal { Id = _goalId, UserId = "test-user-id", Name = "My Goal" },
            new FinancialGoal { Id = Guid.NewGuid(), UserId = "other-user-id", Name = "Other Goal" }
        }.AsQueryable();

        var mockGoals = new Mock<DbSet<FinancialGoal>>().SetupAsQueryable(goals);
        _dbContextMock.Setup(db => db.FinancialGoals).Returns(mockGoals.Object);

        _handler = new GetFinancialGoalByIdQueryHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnGoal_WhenFoundForUser()
    {
        // Arrange
        var query = new GetFinancialGoalByIdQuery(_goalId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_goalId, result.Id);
    }

    [Fact]
    public async Task Handle_Should_ReturnNull_WhenNotFound()
    {
        // Arrange
        var query = new GetFinancialGoalByIdQuery(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
