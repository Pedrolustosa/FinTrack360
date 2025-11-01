using FinTrack360.Application.Features.Budgets.GetBudgets;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using FinTrack360.Tests.Unit.Mocks;

namespace FinTrack360.Tests.Unit.Features.Budgets.GetBudgets;

public class GetBudgetsQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly GetBudgetsQueryHandler _handler;

    public GetBudgetsQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
        }, "mock"));

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var budgets = new List<Budget>
        {
            new Budget { UserId = "test-user-id", Month = 10, Year = 2025, Category = new Category() },
            new Budget { UserId = "test-user-id", Month = 11, Year = 2025, Category = new Category() },
            new Budget { UserId = "test-user-id", Month = 11, Year = 2026, Category = new Category() },
            new Budget { UserId = "other-user-id", Month = 11, Year = 2025, Category = new Category() }
        }.AsQueryable();

        var mockBudgets = new Mock<DbSet<Budget>>().SetupAsQueryable(budgets);
        _dbContextMock.Setup(db => db.Budgets).Returns(mockBudgets.Object);

        _handler = new GetBudgetsQueryHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnAllBudgetsForUser_WhenNoFiltersApplied()
    {
        // Arrange
        var query = new GetBudgetsQuery(null, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task Handle_Should_ReturnFilteredBudgets_WhenMonthFilterApplied()
    {
        // Arrange
        var query = new GetBudgetsQuery(10, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(10, result.First().Month);
    }

    [Fact]
    public async Task Handle_Should_ReturnFilteredBudgets_WhenYearFilterApplied()
    {
        // Arrange
        var query = new GetBudgetsQuery(null, 2026);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(2026, result.First().Year);
    }

    [Fact]
    public async Task Handle_Should_ReturnFilteredBudgets_WhenBothFiltersApplied()
    {
        // Arrange
        var query = new GetBudgetsQuery(11, 2025);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(11, result.First().Month);
        Assert.Equal(2025, result.First().Year);
    }
}
