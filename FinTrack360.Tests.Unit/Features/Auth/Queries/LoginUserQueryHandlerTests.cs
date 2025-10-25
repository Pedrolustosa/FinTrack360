using Moq;
using FluentAssertions;
using FinTrack360.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Application.Features.Auth.LoginUser;
using FinTrack360.Tests.Unit.Mocks;

namespace FinTrack360.Tests.Unit.Features.Auth.Queries;

public class LoginUserQueryHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
    private readonly Mock<IJwtTokenGenerator> _mockJwtGenerator;
    private readonly LoginUserQueryHandler _handler;

    public LoginUserQueryHandlerTests()
    {
        _mockUserManager = MockFactories.GetMockUserManager();
        _mockSignInManager = MockFactories.GetMockSignInManager();
        _mockJwtGenerator = new Mock<IJwtTokenGenerator>();

        _handler = new LoginUserQueryHandler(
            _mockUserManager.Object,
            _mockSignInManager.Object,
            _mockJwtGenerator.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnToken_When_CredentialsAreValid_And_EmailIsConfirmed()
    {
        var query = new LoginUserQuery("test@example.com", "Password123!");
        var user = new ApplicationUser { Id = "123", Email = query.Email };
        var expectedToken = "fake.jwt.token";

        _mockUserManager.Setup(x => x.FindByEmailAsync(query.Email)).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, query.Password, false)).ReturnsAsync(SignInResult.Success);
        _mockJwtGenerator.Setup(x => x.GenerateToken(user)).Returns(expectedToken);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().Be(expectedToken);
        _mockJwtGenerator.Verify(x => x.GenerateToken(user), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_UserNotFound()
    {
        var query = new LoginUserQuery("wrong@example.com", "Password123!");

        _mockUserManager.Setup(x => x.FindByEmailAsync(query.Email)).ReturnsAsync((ApplicationUser)null!);

        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid credentials.");
        _mockJwtGenerator.Verify(x => x.GenerateToken(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_PasswordIsInvalid()
    {
        var query = new LoginUserQuery("test@example.com", "WrongPassword!");
        var user = new ApplicationUser { Id = "123", Email = query.Email };

        _mockUserManager.Setup(x => x.FindByEmailAsync(query.Email)).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, query.Password, false)).ReturnsAsync(SignInResult.Failed);

        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid credentials.");
        _mockJwtGenerator.Verify(x => x.GenerateToken(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_EmailIsNotConfirmed()
    {
        var query = new LoginUserQuery("test@example.com", "Password123!");
        var user = new ApplicationUser { Id = "123", Email = query.Email };

        _mockUserManager.Setup(x => x.FindByEmailAsync(query.Email)).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(false);

        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("Email not confirmed. Please check your inbox for the confirmation link.");
        _mockSignInManager.Verify(x => x.CheckPasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), false), Times.Never);
        _mockJwtGenerator.Verify(x => x.GenerateToken(It.IsAny<ApplicationUser>()), Times.Never);
    }
}