using FinTrack360.Application.Features.Budgets.UpdateBudget;
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

namespace FinTrack360.Tests.Unit.Features.Budgets.UpdateBudget;

public class UpdateBudgetCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly UpdateBudgetCommandHandler _handler;
    private readonly Guid _budgetId;

    public UpdateBudgetCommandHandlerTests()
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
            new Budget { Id = _budgetId, UserId = "test-user-id", Amount = 500 }
        }.AsQueryable();

        var mockBudgets = new Mock<DbSet<Budget>>().SetupAsQueryable(budgets);
        _dbContextMock.Setup(db => db.Budgets).Returns(mockBudgets.Object);

        _handler = new UpdateBudgetCommandHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_UpdateBudgetAmount_WhenFound()
    {
        // Arrange
        var command = new UpdateBudgetCommand(_budgetId, 1000);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedBudget = _dbContextMock.Object.Budgets.First();
        _dbContextMock.Verify(db => db.SaveChangesAsync(CancellationToken.None), Times.Once);
        Assert.Equal(1000, updatedBudget.Amount);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenNotFound()
    {
        // Arrange
        var command = new UpdateBudgetCommand(Guid.NewGuid(), 1000);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
