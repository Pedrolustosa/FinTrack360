using FinTrack360.Application.Features.Budgets.DeleteBudget;
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

namespace FinTrack360.Tests.Unit.Features.Budgets.DeleteBudget;

public class DeleteBudgetCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly DeleteBudgetCommandHandler _handler;
    private readonly Guid _budgetId;

    public DeleteBudgetCommandHandlerTests()
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
            new Budget { Id = _budgetId, UserId = "test-user-id" }
        }.AsQueryable();

        var mockBudgets = new Mock<DbSet<Budget>>().SetupAsQueryable(budgets);
        _dbContextMock.Setup(db => db.Budgets).Returns(mockBudgets.Object);

        _handler = new DeleteBudgetCommandHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_DeleteBudget_WhenFound()
    {
        // Arrange
        var command = new DeleteBudgetCommand(_budgetId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContextMock.Verify(db => db.Budgets.Remove(It.Is<Budget>(b => b.Id == _budgetId)), Times.Once);
        _dbContextMock.Verify(db => db.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenNotFound()
    {
        // Arrange
        var command = new DeleteBudgetCommand(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
