using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace FinTrack360.Application.Features.Auth.RetryConfirmationEmail;

public class RetryConfirmationEmailCommandHandler(UserManager<ApplicationUser> userManager, IEmailService emailService) : IRequestHandler<RetryConfirmationEmailCommand, Unit>
{
    public async Task<Unit> Handle(RetryConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || user.EmailConfirmed)
        {
            return Unit.Value;
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        await emailService.SendConfirmationEmailAsync(user.Email!, user.FirstName, encodedToken, request.Language);

        return Unit.Value;
    }
}
