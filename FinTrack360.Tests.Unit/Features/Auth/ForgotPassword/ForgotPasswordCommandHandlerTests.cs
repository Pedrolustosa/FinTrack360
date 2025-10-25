using Moq;
using FluentAssertions;
using FinTrack360.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Application.Features.Auth.ForgotPassword;
using FinTrack360.Tests.Unit.Mocks;

namespace FinTrack360.Tests.Unit.Features.Auth.ForgotPassword;

public class ForgotPasswordCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly ForgotPasswordCommandHandler _handler;

    public ForgotPasswordCommandHandlerTests()
    {
        _mockUserManager = MockFactories.GetMockUserManager();
        _mockEmailService = new Mock<IEmailService>();
        _handler = new ForgotPasswordCommandHandler(_mockUserManager.Object, _mockEmailService.Object);
    }

    [Fact]
    public async Task Handle_Should_GenerateTokenAndSendEmail_When_UserExists()
    {
        var command = new ForgotPasswordCommand("test@example.com", "en-US");
        var user = new ApplicationUser { Email = command.Email, FirstName = "Test" };
        var fakeToken = "fake-reset-token";

        _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(user))
            .ReturnsAsync(fakeToken);

        _mockEmailService.Setup(x => x.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().NotThrowAsync();
        _mockEmailService.Verify(x => x.SendPasswordResetEmailAsync(user.Email, user.FirstName, It.IsAny<string>(), command.Language), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_DoNothing_When_UserNotFound()
    {
        var command = new ForgotPasswordCommand("wrong@example.com", "en-US");

        _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync((ApplicationUser)null!);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().NotThrowAsync();
        _mockUserManager.Verify(x => x.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()), Times.Never);
        _mockEmailService.Verify(x => x.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}