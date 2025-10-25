using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FinTrack360.Application.Features.Auth.LoginUser;
using FinTrack360.Application.Features.Auth.RegisterUser;
using FinTrack360.Application.Features.Auth.ConfirmEmail;
using FinTrack360.Application.Features.Auth.ResetPassword;
using FinTrack360.Application.Features.Auth.ForgotPassword;
using FinTrack360.Application.Features.Auth.RetryConfirmationEmail;
using FinTrack360.Application.Features.Auth.ChangePassword;
using FinTrack360.Application.Features.Auth.Logout;
using FinTrack360.Application.Common.Interfaces;

namespace FinTrack360.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ISender mediator, IConfiguration configuration, IEmailService emailService) : ControllerBase
{
    private readonly ISender _mediator = mediator;
    private readonly IConfiguration _configuration = configuration;
    private readonly IEmailService _emailService = emailService;

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var userId = await _mediator.Send(command);
        return CreatedAtAction(nameof(Register), new { id = userId }, new { UserId = userId });
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginUserQuery query)
    {
        var token = await _mediator.Send(query);
        return Ok(new { Token = token });
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { Message = "If an account with this email exists, a password reset link has been sent." });
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { Message = "Password has been reset successfully." });
    }

    [HttpGet("confirm-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token, [FromQuery] string lang)
    {
        var command = new ConfirmEmailCommand(email, token);
        await _mediator.Send(command);

        var htmlContent = await _emailService.GetEmailConfirmationPageAsync(lang);

        var clientAppUrl = _configuration.GetValue<string>("ClientAppUrl") ?? "http://localhost:3000";
        htmlContent = htmlContent.Replace("[ClientAppUrl]", clientAppUrl);

        return Content(htmlContent, "text/html");
    }

    [HttpPost("resend-confirmation")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] RetryConfirmationEmailCommand command, [FromHeader(Name = "Accept-Language")] string? language)
    {
        var lang = language?.Split(',').FirstOrDefault()?.Trim() ?? "en-US";
        var localizedCommand = command with { Language = lang };
        await _mediator.Send(localizedCommand);
        return Ok(new { Message = "If an account with this email exists and is not confirmed, a new confirmation email has been sent." });
    }

    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { Message = "Password changed successfully." });
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        await _mediator.Send(new LogoutCommand());
        return Ok(new { Message = "Logged out successfully." });
    }
}