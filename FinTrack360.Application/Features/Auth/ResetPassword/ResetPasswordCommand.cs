using MediatR;
namespace FinTrack360.Application.Features.Auth.ResetPassword;
public record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword) : IRequest<Unit>;