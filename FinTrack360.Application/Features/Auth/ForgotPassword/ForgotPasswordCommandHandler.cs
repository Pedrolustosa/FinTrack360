using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
namespace FinTrack360.Application.Features.Auth.ForgotPassword;
public class ForgotPasswordCommandHandler(UserManager<ApplicationUser> userManager, IEmailService emailService) : IRequestHandler<ForgotPasswordCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IEmailService _emailService = emailService;
    public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Unit.Value;
        }
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        await _emailService.SendPasswordResetEmailAsync(user.Email!, user.FirstName, encodedToken, request.Language);
        return Unit.Value;
    }
}