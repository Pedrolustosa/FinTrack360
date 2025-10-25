using MediatR;
namespace FinTrack360.Application.Features.Auth.ResetPassword;
using FinTrack360.Application.Common.Interfaces;

public record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword) : IRequest<Unit>, IAuditableRequest
{
    public string UserId => Email;
}