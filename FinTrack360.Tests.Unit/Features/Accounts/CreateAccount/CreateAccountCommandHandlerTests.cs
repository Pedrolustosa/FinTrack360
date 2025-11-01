
using FinTrack360.Application.Features.Accounts.CreateAccount;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Tests.Unit.Features.Accounts.CreateAccount;

public class CreateAccountCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock = null!;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = null!;
    private readonly CreateAccountCommandHandler _handler = null!;

    public CreateAccountCommandHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
        }, "mock"));

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var mockAccounts = new Mock<DbSet<Account>>();
        _dbContextMock.Setup(db => db.Accounts).Returns(mockAccounts.Object);

        _handler = new CreateAccountCommandHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_CreateAccountAndReturnId()
    {
        // Arrange
        var command = new CreateAccountCommand("Test Account", Domain.Enums.AccountType.Checking, "Test Bank", 1000);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContextMock.Verify(db => db.Accounts.Add(It.Is<Account>(a => 
            a.UserId == "test-user-id" &&
            a.Name == command.Name &&
            a.Type == command.Type &&
            a.InstitutionName == command.InstitutionName &&
            a.CurrentBalance == command.InitialBalance)), Times.Once);

        _dbContextMock.Verify(db => db.SaveChangesAsync(CancellationToken.None), Times.Once);

        Assert.NotEqual(Guid.Empty, result);
    }
}
