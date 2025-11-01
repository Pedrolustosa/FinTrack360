using FinTrack360.Application.Features.Accounts.GetAccountById;
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

namespace FinTrack360.Tests.Unit.Features.Accounts.GetAccountById;

public class GetAccountByIdQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly GetAccountByIdQueryHandler _handler;

    public GetAccountByIdQueryHandlerTests()
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
            new Account { Id = Guid.NewGuid(), UserId = "test-user-id", Name = "Test Account 1", Type = AccountType.Checking, InstitutionName = "Bank 1", CurrentBalance = 1000 },
            new Account { Id = Guid.NewGuid(), UserId = "other-user-id", Name = "Test Account 2", Type = AccountType.Savings, InstitutionName = "Bank 2", CurrentBalance = 2000 }
        }.AsQueryable();

        var mockSet = new Mock<DbSet<Account>>();
        mockSet.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(accounts.Provider);
        mockSet.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(accounts.Expression);
        mockSet.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(accounts.ElementType);
        mockSet.As<IQueryable<Account>>().Setup(m => m.GetEnumerator()).Returns(accounts.GetEnumerator());

        _dbContextMock.Setup(db => db.Accounts).Returns(mockSet.Object);

        _handler = new GetAccountByIdQueryHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnAccount_WhenFound()
    {
        // Arrange
        var account = _dbContextMock.Object.Accounts.First(a => a.UserId == "test-user-id");
        var query = new GetAccountByIdQuery(account.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(account.Id, result.Id);
    }

    [Fact]
    public async Task Handle_Should_ReturnNull_WhenNotFound()
    {
        // Arrange
        var query = new GetAccountByIdQuery(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
