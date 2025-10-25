using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace FinTrack360.Application.Features.Auth.ResetPassword
{
    public class ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<ResetPasswordCommand, Unit>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email) ?? throw new Exception("Invalid request: User not found.");
            byte[] decodedTokenBytes;
            try
            {
                decodedTokenBytes = WebEncoders.Base64UrlDecode(request.Token);
            }
            catch (FormatException)
            {
                throw new Exception("Invalid token format.");
            }
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Password reset failed: {errors}");
            }
            return Unit.Value;
        }
    }
}