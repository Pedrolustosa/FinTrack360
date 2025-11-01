using FinTrack360.Application.Features.FinancialGoals.CreateFinancialGoal;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Tests.Unit.Features.FinancialGoals.CreateFinancialGoal;

public class CreateFinancialGoalCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly CreateFinancialGoalCommandHandler _handler;

    public CreateFinancialGoalCommandHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
        }, "mock"));

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var mockGoals = new Mock<DbSet<FinancialGoal>>();
        _dbContextMock.Setup(db => db.FinancialGoals).Returns(mockGoals.Object);

        _handler = new CreateFinancialGoalCommandHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_CreateFinancialGoal_AndReturnId()
    {
        // Arrange
        var command = new CreateFinancialGoalCommand("Vacation Fund", 5000, DateTime.UtcNow.AddYears(1));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContextMock.Verify(db => db.FinancialGoals.Add(It.Is<FinancialGoal>(g => 
            g.UserId == "test-user-id" &&
            g.Name == "Vacation Fund" &&
            g.TargetAmount == 5000 &&
            g.CurrentAmount == 0
        )), Times.Once);
        _dbContextMock.Verify(db => db.SaveChangesAsync(CancellationToken.None), Times.Once);
        Assert.NotEqual(Guid.Empty, result);
    }
}
