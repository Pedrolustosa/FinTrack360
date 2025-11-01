using FinTrack360.Application.Features.Reports.GetNetWorth;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FinTrack360.Tests.Unit.Mocks;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Tests.Unit.Features.Reports.GetNetWorth;

public class GetNetWorthQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly GetNetWorthQueryHandler _handler;

    public GetNetWorthQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "test-user-id") }, "mock"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var userAccount1 = new Account { Id = Guid.NewGuid(), UserId = "test-user-id", CurrentBalance = 1000 };
        var userAccount2 = new Account { Id = Guid.NewGuid(), UserId = "test-user-id", CurrentBalance = -500 }; // Credit Card
        var otherUserAccount = new Account { Id = Guid.NewGuid(), UserId = "other-user-id", CurrentBalance = 5000 };

        var accounts = new List<Account> { userAccount1, userAccount2, otherUserAccount }.AsQueryable();

        var assets = new List<Asset>
        {
            new Asset { Account = userAccount1, Quantity = 10, AverageCost = 150 }, // Value = 1500
            new Asset { Account = userAccount1, Quantity = 5, AverageCost = 200 }, // Value = 1000
            new Asset { Account = otherUserAccount, Quantity = 1, AverageCost = 999 } // Should be ignored
        }.AsQueryable();

        var mockAccounts = new Mock<DbSet<Account>>().SetupAsQueryable(accounts);
        var mockAssets = new Mock<DbSet<Asset>>().SetupAsQueryable(assets);

        _dbContextMock.Setup(db => db.Accounts).Returns(mockAccounts.Object);
        _dbContextMock.Setup(db => db.Assets).Returns(mockAssets.Object);

        _handler = new GetNetWorthQueryHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_CalculateNetWorthCorrectly()
    {
        // Arrange
        var query = new GetNetWorthQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        // Total Account Balance = 1000 - 500 = 500
        // Total Asset Value = (10 * 150) + (5 * 200) = 1500 + 1000 = 2500
        Assert.Equal(500, result.TotalAccountBalance);
        Assert.Equal(2500, result.TotalAssetValue);
    }
}
