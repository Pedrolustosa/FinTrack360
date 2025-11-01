using FinTrack360.Application.Features.Profile.UpdateProfile;
using FinTrack360.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FinTrack360.Tests.Unit.Features.Profile.UpdateProfile;

public class UpdateProfileCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly UpdateProfileCommandHandler _handler;

    public UpdateProfileCommandHandlerTests()
    {
        // Mocking UserManager is complex, so we need a mock store.
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

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "test-user-id") }, "mock"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        _handler = new UpdateProfileCommandHandler(_userManagerMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_UpdateUserProfile_WhenUserExists()
    {
        // Arrange
        var user = new ApplicationUser { Id = "test-user-id", FirstName = "Old", LastName = "Name" };
        _userManagerMock.Setup(um => um.FindByIdAsync("test-user-id")).ReturnsAsync(user);
        _userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

        var command = new UpdateProfileCommand("New", "User", "1234567890");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userManagerMock.Verify(um => um.UpdateAsync(It.Is<ApplicationUser>(u => 
            u.FirstName == "New" && 
            u.LastName == "User" && 
            u.PhoneNumber == "1234567890"
        )), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenUserNotFound()
    {
        // Arrange
        _userManagerMock.Setup(um => um.FindByIdAsync("test-user-id")).ReturnsAsync((ApplicationUser)null);
        var command = new UpdateProfileCommand("New", "User", "1234567890");

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenUpdateFails()
    {
        // Arrange
        var user = new ApplicationUser { Id = "test-user-id" };
        _userManagerMock.Setup(um => um.FindByIdAsync("test-user-id")).ReturnsAsync(user);
        _userManagerMock.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));
        var command = new UpdateProfileCommand("New", "User", "1234567890");

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
