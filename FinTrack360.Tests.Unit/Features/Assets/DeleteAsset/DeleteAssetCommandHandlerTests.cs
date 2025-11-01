using FinTrack360.Application.Features.Assets.DeleteAsset;
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

namespace FinTrack360.Tests.Unit.Features.Assets.DeleteAsset;

public class DeleteAssetCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly DeleteAssetCommandHandler _handler;
    private readonly Guid _accountId;
    private readonly Guid _assetId;

    public DeleteAssetCommandHandlerTests()
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
            new Asset { Id = _assetId, AccountId = _accountId }
        }.AsQueryable();

        var mockAccounts = new Mock<DbSet<Account>>().SetupAsQueryable(accounts);
        var mockAssets = new Mock<DbSet<Asset>>().SetupAsQueryable(assets);

        _dbContextMock.Setup(db => db.Accounts).Returns(mockAccounts.Object);
        _dbContextMock.Setup(db => db.Assets).Returns(mockAssets.Object);

        _handler = new DeleteAssetCommandHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_DeleteAsset_WhenFound()
    {
        // Arrange
        var command = new DeleteAssetCommand(_accountId, _assetId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContextMock.Verify(db => db.Assets.Remove(It.Is<Asset>(a => a.Id == _assetId)), Times.Once);
        _dbContextMock.Verify(db => db.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenAccountNotFound()
    {
        // Arrange
        var command = new DeleteAssetCommand(Guid.NewGuid(), _assetId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenAssetNotFound()
    {
        // Arrange
        var command = new DeleteAssetCommand(_accountId, Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
