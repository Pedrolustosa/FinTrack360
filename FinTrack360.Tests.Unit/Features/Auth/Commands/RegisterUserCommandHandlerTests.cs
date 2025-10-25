using Moq;
using FluentAssertions;
using FinTrack360.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Application.Features.Auth.RegisterUser;
using FinTrack360.Tests.Unit.Mocks;

namespace FinTrack360.Tests.Unit.Features.Auth.Commands;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _mockUserManager = MockFactories.GetMockUserManager();
        _mockEmailService = new Mock<IEmailService>();
        _handler = new RegisterUserCommandHandler(_mockUserManager.Object, _mockEmailService.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnUserId_And_SendEmail_When_UserIsCreatedSuccessfully()
    {
        var command = new RegisterUserCommand(
            "Test", "User", "test@example.com", "Password123!", "en-US");
        var fakeToken = "fake-confirmation-token";

        _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync((ApplicationUser)null!);

        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), command.Password))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(fakeToken);

        _mockEmailService.Setup(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNullOrEmpty();
        _mockUserManager.Verify(x => x.CreateAsync(It.Is<ApplicationUser>(u => u.Email == command.Email), command.Password), Times.Once);
        _mockEmailService.Verify(x => x.SendConfirmationEmailAsync(command.Email, command.FirstName, It.IsAny<string>(), command.Language), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_UserAlreadyExists()
    {
        var command = new RegisterUserCommand(
            "Test", "User", "test@example.com", "Password123!", "en-US");

        _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync(new ApplicationUser());

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("User with this email already exists.");

        _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        _mockEmailService.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_CreateAsyncFails()
    {
        var command = new RegisterUserCommand(
            "Test", "User", "test@example.com", "Password123!", "en-US");

        var failedResult = IdentityResult.Failed(new IdentityError { Description = "Password is too weak." });

        _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync((ApplicationUser)null!);

        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), command.Password))
            .ReturnsAsync(failedResult);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*Password is too weak.*");

        _mockEmailService.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}