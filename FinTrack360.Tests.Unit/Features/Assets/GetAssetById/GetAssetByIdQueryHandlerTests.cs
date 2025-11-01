using FinTrack360.Application.Features.Assets.GetAssetById;
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

namespace FinTrack360.Tests.Unit.Features.Assets.GetAssetById;

public class GetAssetByIdQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly GetAssetByIdQueryHandler _handler;
    private readonly Guid _accountId;
    private readonly Guid _assetId;

    public GetAssetByIdQueryHandlerTests()
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
            new Asset { Id = _assetId, AccountId = _accountId, Ticker = "AAPL" }
        }.AsQueryable();

        var mockAccounts = new Mock<DbSet<Account>>();
        mockAccounts.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(accounts.Provider);
        mockAccounts.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(accounts.Expression);
        mockAccounts.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(accounts.ElementType);
        mockAccounts.As<IQueryable<Account>>().Setup(m => m.GetEnumerator()).Returns(accounts.GetEnumerator());

        var mockAssets = new Mock<DbSet<Asset>>();
        mockAssets.As<IQueryable<Asset>>().Setup(m => m.Provider).Returns(assets.Provider);
        mockAssets.As<IQueryable<Asset>>().Setup(m => m.Expression).Returns(assets.Expression);
        mockAssets.As<IQueryable<Asset>>().Setup(m => m.ElementType).Returns(assets.ElementType);
        mockAssets.As<IQueryable<Asset>>().Setup(m => m.GetEnumerator()).Returns(assets.GetEnumerator());

        _dbContextMock.Setup(db => db.Accounts).Returns(mockAccounts.Object);
        _dbContextMock.Setup(db => db.Assets).Returns(mockAssets.Object);

        _handler = new GetAssetByIdQueryHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnAsset_WhenFound()
    {
        // Arrange
        var query = new GetAssetByIdQuery(_accountId, _assetId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_assetId, result.Id);
    }

    [Fact]
    public async Task Handle_Should_ReturnNull_WhenAssetNotFound()
    {
        // Arrange
        var query = new GetAssetByIdQuery(_accountId, Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_Should_ReturnNull_WhenAccountNotFound()
    {
        // Arrange
        var query = new GetAssetByIdQuery(Guid.NewGuid(), _assetId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
