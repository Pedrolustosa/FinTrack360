using FinTrack360.Application.Features.Dashboard.AccountSummary;
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

namespace FinTrack360.Tests.Unit.Features.Dashboard.AccountSummary;

public class GetAccountSummaryQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly GetAccountSummaryQueryHandler _handler;

    public GetAccountSummaryQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();

        var accounts = new List<Account>
        {
            new Account { UserId = "test-user-id", Name = "Savings", Type = AccountType.Savings, InstitutionName = "Bank A", CurrentBalance = 5000 },
            new Account { UserId = "test-user-id", Name = "Checking", Type = AccountType.Checking, InstitutionName = "Bank B", CurrentBalance = 1000 },
            new Account { UserId = "other-user-id", Name = "Investment", Type = AccountType.Investment, InstitutionName = "Broker C", CurrentBalance = 10000 }
        }.AsQueryable();

        var mockAccounts = new Mock<DbSet<Account>>().SetupAsQueryable(accounts);
        _dbContextMock.Setup(db => db.Accounts).Returns(mockAccounts.Object);

        _handler = new GetAccountSummaryQueryHandler(_dbContextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnAccountSummariesForUser_OrderedByType()
    {
        // Arrange
        var query = new GetAccountSummaryQuery("test-user-id");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Checking", result[0].Name);
        Assert.Equal("Savings", result[1].Name);
        Assert.Equal(1000, result[0].CurrentBalance);
    }
}
