using FinTrack360.Application.Features.Budgets.CreateBudget;
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

namespace FinTrack360.Tests.Unit.Features.Budgets.CreateBudget;

public class CreateBudgetCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly CreateBudgetCommandHandler _handler;

    public CreateBudgetCommandHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
        }, "mock"));

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var budgets = new List<Budget>().AsQueryable();
        var mockBudgets = new Mock<DbSet<Budget>>().SetupAsQueryable(budgets);
        _dbContextMock.Setup(db => db.Budgets).Returns(mockBudgets.Object);

        _handler = new CreateBudgetCommandHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_CreateBudget_WhenBudgetDoesNotExist()
    {
        // Arrange
        var command = new CreateBudgetCommand(Guid.NewGuid(), 500, 11, 2025);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContextMock.Verify(db => db.Budgets.Add(It.IsAny<Budget>()), Times.Once);
        _dbContextMock.Verify(db => db.SaveChangesAsync(CancellationToken.None), Times.Once);
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenBudgetAlreadyExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingBudgets = new List<Budget>
        {
            new Budget { UserId = "test-user-id", CategoryId = categoryId, Month = 11, Year = 2025, Amount = 500 }
        }.AsQueryable();

        var mockBudgets = new Mock<DbSet<Budget>>().SetupAsQueryable(existingBudgets);
        _dbContextMock.Setup(db => db.Budgets).Returns(mockBudgets.Object);

        var command = new CreateBudgetCommand(categoryId, 600, 11, 2025);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
