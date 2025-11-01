using FinTrack360.Application.Features.Profile.DeleteAccount;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace FinTrack360.Tests.Unit.Features.Profile.DeleteAccount;

public class DeleteAccountCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<ITokenRevocationService> _tokenRevocationServiceMock;
    private readonly DeleteAccountCommandHandler _handler;

    public DeleteAccountCommandHandlerTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, 
            new Mock<IOptions<IdentityOptions>>().Object, 
            new Mock<IPasswordHasher<ApplicationUser>>().Object, 
            new IUserValidator<ApplicationUser>[0], 
            new IPasswordValidator<ApplicationUser>[0], 
            new Mock<ILookupNormalizer>().Object, 
            new Mock<IdentityErrorDescriber>().Object, 
            new Mock<IServiceProvider>().Object, 
            new Mock<ILogger<UserManager<ApplicationUser>>>().Object
        );
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _tokenRevocationServiceMock = new Mock<ITokenRevocationService>();

        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString())
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims, "mock"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        _handler = new DeleteAccountCommandHandler(_userManagerMock.Object, _httpContextAccessorMock.Object, _tokenRevocationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_SoftDeleteUserAndRevokeToken_WhenPasswordIsCorrect()
    {
        // Arrange
        var user = new ApplicationUser { Id = "test-user-id" };
        _userManagerMock.Setup(um => um.FindByIdAsync("test-user-id")).ReturnsAsync(user);
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, "password123")).ReturnsAsync(true);
        _userManagerMock.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        var command = new DeleteAccountCommand("password123");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userManagerMock.Verify(um => um.UpdateAsync(It.Is<ApplicationUser>(u => u.IsDeleted && u.DeletedAt.HasValue)), Times.Once);
        _tokenRevocationServiceMock.Verify(trs => trs.RevokeTokenAsync(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenPasswordIsIncorrect()
    {
        // Arrange
        var user = new ApplicationUser { Id = "test-user-id" };
        _userManagerMock.Setup(um => um.FindByIdAsync("test-user-id")).ReturnsAsync(user);
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, "wrongpassword")).ReturnsAsync(false);

        var command = new DeleteAccountCommand("wrongpassword");

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
