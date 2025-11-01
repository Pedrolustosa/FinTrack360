using FinTrack360.Application.Features.FinancialGoals.GetFinancialGoals;
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

namespace FinTrack360.Tests.Unit.Features.FinancialGoals.GetFinancialGoals;

public class GetFinancialGoalsQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly GetFinancialGoalsQueryHandler _handler;

    public GetFinancialGoalsQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
        }, "mock"));

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var goals = new List<FinancialGoal>
        {
            new FinancialGoal { UserId = "test-user-id", Name = "Goal 2", TargetDate = DateTime.UtcNow.AddYears(2) },
            new FinancialGoal { UserId = "test-user-id", Name = "Goal 1", TargetDate = DateTime.UtcNow.AddYears(1) },
            new FinancialGoal { UserId = "other-user-id", Name = "Other User Goal", TargetDate = DateTime.UtcNow.AddMonths(6) }
        }.AsQueryable();

        var mockGoals = new Mock<DbSet<FinancialGoal>>().SetupAsQueryable(goals);
        _dbContextMock.Setup(db => db.FinancialGoals).Returns(mockGoals.Object);

        _handler = new GetFinancialGoalsQueryHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnUserGoals_OrderedByTargetDate()
    {
        // Arrange
        var query = new GetFinancialGoalsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal("Goal 1", resultList[0].Name);
        Assert.Equal("Goal 2", resultList[1].Name);
    }
}
