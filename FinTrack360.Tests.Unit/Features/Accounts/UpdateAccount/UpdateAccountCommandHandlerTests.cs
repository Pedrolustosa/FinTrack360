using FinTrack360.Application.Features.Accounts.UpdateAccount;
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

namespace FinTrack360.Tests.Unit.Features.Accounts.UpdateAccount;

public class UpdateAccountCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly UpdateAccountCommandHandler _handler;

    public UpdateAccountCommandHandlerTests()
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
            new Account { Id = Guid.NewGuid(), UserId = "test-user-id", Name = "Test Account 1", Type = AccountType.Checking, InstitutionName = "Bank 1", CurrentBalance = 1000 }
        }.AsQueryable();

        var mockSet = new Mock<DbSet<Account>>();
        mockSet.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(accounts.Provider);
        mockSet.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(accounts.Expression);
        mockSet.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(accounts.ElementType);
        mockSet.As<IQueryable<Account>>().Setup(m => m.GetEnumerator()).Returns(accounts.GetEnumerator());

        _dbContextMock.Setup(db => db.Accounts).Returns(mockSet.Object);

        _handler = new UpdateAccountCommandHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_UpdateAccount_WhenFound()
    {
        // Arrange
        var account = _dbContextMock.Object.Accounts.First();
        var command = new UpdateAccountCommand(account.Id, "Updated Name", AccountType.Savings, "Updated Bank", 2000);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContextMock.Verify(db => db.SaveChangesAsync(CancellationToken.None), Times.Once);
        Assert.Equal("Updated Name", account.Name);
        Assert.Equal(AccountType.Savings, account.Type);
        Assert.Equal("Updated Bank", account.InstitutionName);
        Assert.Equal(2000, account.CurrentBalance);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenNotFound()
    {
        // Arrange
        var command = new UpdateAccountCommand(Guid.NewGuid(), "Updated Name", AccountType.Savings, "Updated Bank", 2000);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
