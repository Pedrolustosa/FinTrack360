using FinTrack360.Application.Features.Assets.UpdateAsset;
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
using FinTrack360.Tests.Unit.Mocks;

namespace FinTrack360.Tests.Unit.Features.Assets.UpdateAsset;

public class UpdateAssetCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly UpdateAssetCommandHandler _handler;
    private readonly Guid _accountId;
    private readonly Guid _assetId;

    public UpdateAssetCommandHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
        }, "mock"));

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        _accountId = Guid.NewGuid();
        _assetId = Guid.NewGuid();

        var accounts = new List<Account>
        {
            new Account { Id = _accountId, UserId = "test-user-id", Type = AccountType.Investment }
        }.AsQueryable();

        var assets = new List<Asset>
        {
            new Asset { Id = _assetId, AccountId = _accountId, Ticker = "AAPL", Name = "Apple Inc.", Quantity = 10, AverageCost = 150.0m }
        }.AsQueryable();

        var mockAccounts = new Mock<DbSet<Account>>().SetupAsQueryable(accounts);
        var mockAssets = new Mock<DbSet<Asset>>().SetupAsQueryable(assets);

        _dbContextMock.Setup(db => db.Accounts).Returns(mockAccounts.Object);
        _dbContextMock.Setup(db => db.Assets).Returns(mockAssets.Object);

        _handler = new UpdateAssetCommandHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_UpdateAsset_WhenFound()
    {
        // Arrange
        var command = new UpdateAssetCommand(_accountId, _assetId, "MSFT", "Microsoft Corp.", 20, 300.0m);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedAsset = _dbContextMock.Object.Assets.First();
        _dbContextMock.Verify(db => db.SaveChangesAsync(CancellationToken.None), Times.Once);
        Assert.Equal("MSFT", updatedAsset.Ticker);
        Assert.Equal("Microsoft Corp.", updatedAsset.Name);
        Assert.Equal(20, updatedAsset.Quantity);
        Assert.Equal(300.0m, updatedAsset.AverageCost);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenAccountNotFound()
    {
        // Arrange
        var command = new UpdateAssetCommand(Guid.NewGuid(), _assetId, "MSFT", "Microsoft Corp.", 20, 300.0m);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenAssetNotFound()
    {
        // Arrange
        var command = new UpdateAssetCommand(_accountId, Guid.NewGuid(), "MSFT", "Microsoft Corp.", 20, 300.0m);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
