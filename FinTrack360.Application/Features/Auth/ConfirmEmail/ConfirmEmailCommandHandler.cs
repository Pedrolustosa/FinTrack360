using MediatR;
using System.Text;
using FinTrack360.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace FinTrack360.Application.Features.Auth.ConfirmEmail;

public class ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<ConfirmEmailCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    public async Task<Unit> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email) ?? throw new Exception("Invalid confirmation request.");
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
        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Email confirmation failed: {errors}");
        }
        return Unit.Value;
    }
}