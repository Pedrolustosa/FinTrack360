using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FinTrack360.Application.Features.Profile.UpdateProfile;

public class UpdateProfileCommandHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateProfileCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            throw new Exception("User not found.");
        }

        var user = await userManager.FindByIdAsync(userId) ?? throw new Exception("User not found.");
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Profile update failed: {errors}");
        }

        return Unit.Value;
    }
}
