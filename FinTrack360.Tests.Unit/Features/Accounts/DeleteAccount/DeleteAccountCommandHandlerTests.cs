using FinTrack360.Application.Features.Accounts.DeleteAccount;
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

namespace FinTrack360.Tests.Unit.Features.Accounts.DeleteAccount;

public class DeleteAccountCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly DeleteAccountCommandHandler _handler;

    public DeleteAccountCommandHandlerTests()
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

        _handler = new DeleteAccountCommandHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_DeleteAccount_WhenFound()
    {
        // Arrange
        var account = _dbContextMock.Object.Accounts.First();
        var command = new DeleteAccountCommand(account.Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContextMock.Verify(db => db.Accounts.Remove(account), Times.Once);
        _dbContextMock.Verify(db => db.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenNotFound()
    {
        // Arrange
        var command = new DeleteAccountCommand(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
