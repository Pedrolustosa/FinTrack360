using Moq;
using FluentAssertions;
using FinTrack360.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using FinTrack360.Application.Features.Auth.ConfirmEmail;
using FinTrack360.Tests.Unit.Mocks;

namespace FinTrack360.Tests.Unit.Features.Auth.Commands;

public class ConfirmEmailCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly ConfirmEmailCommandHandler _handler;

    public ConfirmEmailCommandHandlerTests()
    {
        _mockUserManager = MockFactories.GetMockUserManager();
        _handler = new ConfirmEmailCommandHandler(_mockUserManager.Object);
    }

    [Fact]
    public async Task Handle_Should_ConfirmEmail_When_TokenIsValid()
    {
        var encodedToken = "ZHVtbXktdG9rZW4=";
        var command = new ConfirmEmailCommand("test@example.com", encodedToken);
        var user = new ApplicationUser { Email = command.Email };

        _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _mockUserManager.Setup(x => x.ConfirmEmailAsync(user, "dummy-token"))
            .ReturnsAsync(IdentityResult.Success);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().NotThrowAsync();
        _mockUserManager.Verify(x => x.ConfirmEmailAsync(user, "dummy-token"), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_UserNotFound()
    {
        var command = new ConfirmEmailCommand("wrong@example.com", "token");

        _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync((ApplicationUser)null!);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid confirmation request.");
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_TokenIsInvalid()
    {
        var command = new ConfirmEmailCommand("test@example.com", "invalid-base64-token-!!");

        _mockUserManager.Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync(new ApplicationUser());

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid token format.");
    }
}