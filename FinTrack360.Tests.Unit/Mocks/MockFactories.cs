using FinTrack360.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace FinTrack360.Tests.Unit.Mocks;
public static class MockFactories
{
    public static Mock<UserManager<ApplicationUser>> GetMockUserManager()
    {
        var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(
            mockUserStore.Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<ApplicationUser>>().Object,
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<ApplicationUser>>>().Object);
    }
    public static Mock<SignInManager<ApplicationUser>> GetMockSignInManager()
    {
        var mockUserManager = GetMockUserManager();
        return new Mock<SignInManager<ApplicationUser>>(
            mockUserManager.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<ILogger<SignInManager<ApplicationUser>>>(),
            Mock.Of<IAuthenticationSchemeProvider>(),
            Mock.Of<IUserConfirmation<ApplicationUser>>());
    }
}
