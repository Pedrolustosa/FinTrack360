using FinTrack360.Application.Features.Dashboard.KPIs.NetWorth;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using FinTrack360.Domain.Enums;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FinTrack360.Tests.Unit.Mocks;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Tests.Unit.Features.Dashboard.KPIs.NetWorth;

public class GetNetWorthQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly GetNetWorthQueryHandler _handler;

    public GetNetWorthQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();

        var investmentAccountId = Guid.NewGuid();

        var accounts = new List<Account>
        {
            new Account { UserId = "test-user-id", Type = AccountType.Checking, CurrentBalance = 1000 },
            new Account { UserId = "test-user-id", Type = AccountType.Savings, CurrentBalance = 5000 },
            new Account { UserId = "test-user-id", Id = investmentAccountId, Type = AccountType.Investment, CurrentBalance = 0 }, // Balance from assets
            new Account { UserId = "test-user-id", Type = AccountType.CreditCard, CurrentBalance = -500 } // Liability
        }.AsQueryable();

        var assets = new List<Asset>
        {
            new Asset { AccountId = investmentAccountId, Quantity = 10, AverageCost = 150 } // 1500
        }.AsQueryable();

        var debts = new List<Debt>
        {
            new Debt { UserId = "test-user-id", CurrentBalance = 20000 } // Liability
        }.AsQueryable();

        var mockAccounts = new Mock<DbSet<Account>>().SetupAsQueryable(accounts);
        var mockAssets = new Mock<DbSet<Asset>>().SetupAsQueryable(assets);
        var mockDebts = new Mock<DbSet<Debt>>().SetupAsQueryable(debts);

        _dbContextMock.Setup(db => db.Accounts).Returns(mockAccounts.Object);
        _dbContextMock.Setup(db => db.Assets).Returns(mockAssets.Object);
        _dbContextMock.Setup(db => db.Debts).Returns(mockDebts.Object);

        _handler = new GetNetWorthQueryHandler(_dbContextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_CalculateNetWorthCorrectly()
    {
        // Arrange
        var query = new GetNetWorthQuery("test-user-id");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        // Total Assets = 1000 (Checking) + 5000 (Savings) + 1500 (Investment Assets) = 7500
        // Total Liabilities = 500 (Credit Card) + 20000 (Debt) = 20500
        // Net Worth = 7500 - 20500 = -13000
        Assert.Equal(7500, result.TotalAssets);
        Assert.Equal(20500, result.TotalLiabilities);
        Assert.Equal(-13000, result.NetWorth);
    }
}
