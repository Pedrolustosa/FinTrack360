using FinTrack360.Application.Features.Assets.GetAssets;
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

namespace FinTrack360.Tests.Unit.Features.Assets.GetAssets;

public class GetAssetsQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly GetAssetsQueryHandler _handler;
    private readonly Guid _accountId;

    public GetAssetsQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
        }, "mock"));

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        _accountId = Guid.NewGuid();

        var accounts = new List<Account>
        {
            new Account { Id = _accountId, UserId = "test-user-id", Type = AccountType.Investment }
        }.AsQueryable();

        var assets = new List<Asset>
        {
            new Asset { Id = Guid.NewGuid(), AccountId = _accountId, Name = "Asset B" },
            new Asset { Id = Guid.NewGuid(), AccountId = _accountId, Name = "Asset A" },
            new Asset { Id = Guid.NewGuid(), AccountId = Guid.NewGuid(), Name = "Asset C" }
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

        _handler = new GetAssetsQueryHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnAssetsForAccount_WhenAccountExists()
    {
        // Arrange
        var query = new GetAssetsQuery(_accountId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("Asset A", result.First().Name); // Should be ordered by name
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenAccountNotFound()
    {
        // Arrange
        var query = new GetAssetsQuery(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }
}
