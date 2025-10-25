using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
namespace FinTrack360.Application.Features.Auth.RegisterUser;
public class RegisterUserCommandHandler(UserManager<ApplicationUser> userManager, IEmailService emailService) : IRequestHandler<RegisterUserCommand, string>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IEmailService _emailService = emailService;
    public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _userManager.FindByEmailAsync(request.Email);
        if (userExists != null)
        {
            throw new Exception("User with this email already exists.");
        }
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email
        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"User creation failed: {errors}");
        }
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        await _emailService.SendConfirmationEmailAsync(user.Email!, user.FirstName, encodedToken, request.Language);
        return user.Id;
    }
}