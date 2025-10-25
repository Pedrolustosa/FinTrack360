using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinTrack360.Application.Features.Profile.DeleteAccount;

public class DeleteAccountCommandHandler(
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor,
    ITokenRevocationService tokenRevocationService) : IRequestHandler<DeleteAccountCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not found.");
        var user = await userManager.FindByIdAsync(userId) ?? throw new Exception("User not found.");

        var passwordCorrect = await userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordCorrect)
        {
            throw new Exception("Incorrect password.");
        }

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to delete account: {errors}");
        }

        var jti = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        var exp = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;

        if (jti is not null && long.TryParse(exp, out var expSeconds))
        {
            var expiration = DateTimeOffset.FromUnixTimeSeconds(expSeconds).DateTime;
            await tokenRevocationService.RevokeTokenAsync(jti, expiration);
        }

        return Unit.Value;
    }
}
