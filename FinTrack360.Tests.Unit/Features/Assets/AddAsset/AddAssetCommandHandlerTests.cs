using FinTrack360.Application.Features.Assets.AddAsset;
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
using FinTrack360.Domain.Enums;

namespace FinTrack360.Tests.Unit.Features.Assets.AddAsset;

public class AddAssetCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly AddAssetCommandHandler _handler;

    public AddAssetCommandHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
        }, "mock"));

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var accounts = new List<Account>
        {
            new Account { Id = Guid.NewGuid(), UserId = "test-user-id", Name = "Investment Account", Type = AccountType.Investment },
            new Account { Id = Guid.NewGuid(), UserId = "test-user-id", Name = "Checking Account", Type = AccountType.Checking }
        }.AsQueryable();

        var mockAccounts = new Mock<DbSet<Account>>();
        mockAccounts.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(accounts.Provider);
        mockAccounts.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(accounts.Expression);
        mockAccounts.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(accounts.ElementType);
        mockAccounts.As<IQueryable<Account>>().Setup(m => m.GetEnumerator()).Returns(accounts.GetEnumerator());

        _dbContextMock.Setup(db => db.Accounts).Returns(mockAccounts.Object);

        var mockAssets = new Mock<DbSet<Asset>>();
        _dbContextMock.Setup(db => db.Assets).Returns(mockAssets.Object);

        _handler = new AddAssetCommandHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_AddAsset_WhenAccountIsInvestmentType()
    {
        // Arrange
        var account = _dbContextMock.Object.Accounts.First(a => a.Type == AccountType.Investment);
        var command = new AddAssetCommand(account.Id, "AAPL", "Apple Inc.", 10, 150.0m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContextMock.Verify(db => db.Assets.Add(It.IsAny<Asset>()), Times.Once);
        _dbContextMock.Verify(db => db.SaveChangesAsync(CancellationToken.None), Times.Once);
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenAccountNotFound()
    {
        // Arrange
        var command = new AddAssetCommand(Guid.NewGuid(), "AAPL", "Apple Inc.", 10, 150.0m);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenAccountIsNotInvestmentType()
    {
        // Arrange
        var account = _dbContextMock.Object.Accounts.First(a => a.Type == AccountType.Checking);
        var command = new AddAssetCommand(account.Id, "AAPL", "Apple Inc.", 10, 150.0m);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
