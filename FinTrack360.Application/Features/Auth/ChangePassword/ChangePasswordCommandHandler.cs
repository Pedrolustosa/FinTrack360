using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FinTrack360.Application.Features.Auth.ChangePassword;

public class ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor) : IRequestHandler<ChangePasswordCommand, Unit>
{
    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (request.NewPassword != request.ConfirmPassword)
        {
            throw new Exception("New password and confirmation password do not match.");
        }

        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            throw new Exception("User not found.");
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var result = await userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Password change failed: {errors}");
        }

        return Unit.Value;
    }
}
