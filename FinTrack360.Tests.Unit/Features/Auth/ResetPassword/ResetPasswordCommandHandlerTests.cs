using Moq;
using FluentAssertions;
using FinTrack360.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using FinTrack360.Application.Features.Auth.ResetPassword;
using FinTrack360.Tests.Unit.Mocks;

namespace FinTrack360.Tests.Unit.Features.Auth.ResetPassword;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly ResetPasswordCommandHandler _handler;

    public ResetPasswordCommandHandlerTests()
    {
        _mockUserManager = MockFactories.GetMockUserManager();
        _handler = new ResetPasswordCommandHandler(_mockUserManager.Object);
    }

    [Fact]
    public async Task Handle_Should_ResetPassword_When_TokenIsValid()
    {
        var encodedToken = "ZHVtbXktdG9rZW4=";
        var command = new ResetPasswordCommand("test@example.com", encodedToken, "NewPassword123!");
        var user = new ApplicationUser { Email = command.Email };

        _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.ResetPasswordAsync(user, "dummy-token", command.NewPassword)).ReturnsAsync(IdentityResult.Success);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().NotThrowAsync();
        _mockUserManager.Verify(x => x.ResetPasswordAsync(user, "dummy-token", command.NewPassword), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_ResetFails()
    {
        var encodedToken = "ZHVtbXktdG9rZW4=";
        var command = new ResetPasswordCommand("test@example.com", encodedToken, "weak");
        var user = new ApplicationUser { Email = command.Email };
        var failedResult = IdentityResult.Failed(new IdentityError { Description = "Password too weak" });

        _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.ResetPasswordAsync(user, "dummy-token", command.NewPassword)).ReturnsAsync(failedResult);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("*Password too weak*");
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_UserNotFound()
    {
        var command = new ResetPasswordCommand("wrong@example.com", "token", "NewPassword123!");

        _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync((ApplicationUser)null!);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid request: User not found.");
    }
}