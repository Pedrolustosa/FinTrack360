using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
namespace FinTrack360.Application.Features.Auth.LoginUser;
public class LoginUserQueryHandler(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<LoginUserQuery, string>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    public async Task<string> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email) ?? throw new Exception("Invalid credentials.");

        if (user.IsDeleted)
        {
            throw new Exception("Invalid credentials.");
        }
        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            throw new Exception("Email not confirmed. Please check your inbox for the confirmation link.");
        }
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            throw new Exception("Invalid credentials.");
        }
        var token = _jwtTokenGenerator.GenerateToken(user);
        return token;
    }
}