using FinTrack360.Application.Features.Budgets.GetBudgetById;
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

namespace FinTrack360.Tests.Unit.Features.Budgets.GetBudgetById;

public class GetBudgetByIdQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly GetBudgetByIdQueryHandler _handler;
    private readonly Guid _budgetId;

    public GetBudgetByIdQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
        }, "mock"));

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        _budgetId = Guid.NewGuid();

        var budgets = new List<Budget>
        {
            new Budget { Id = _budgetId, UserId = "test-user-id", CategoryId = Guid.NewGuid(), Amount = 500, Month = 11, Year = 2025, Category = new Category { Name = "Groceries" } },
            new Budget { Id = Guid.NewGuid(), UserId = "other-user-id", CategoryId = Guid.NewGuid(), Amount = 1000, Month = 11, Year = 2025 }
        }.AsQueryable();

        var mockBudgets = new Mock<DbSet<Budget>>().SetupAsQueryable(budgets);
        _dbContextMock.Setup(db => db.Budgets).Returns(mockBudgets.Object);

        _handler = new GetBudgetByIdQueryHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnBudget_WhenFound()
    {
        // Arrange
        var query = new GetBudgetByIdQuery(_budgetId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_budgetId, result.Id);
        Assert.NotNull(result.Category);
        Assert.Equal("Groceries", result.Category.Name);
    }

    [Fact]
    public async Task Handle_Should_ReturnNull_WhenNotFound()
    {
        // Arrange
        var query = new GetBudgetByIdQuery(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
