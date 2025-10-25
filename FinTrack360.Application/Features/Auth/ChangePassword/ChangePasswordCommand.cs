using MediatR;

namespace FinTrack360.Application.Features.Auth.ChangePassword;

public record ChangePasswordCommand(string OldPassword, string NewPassword, string ConfirmPassword) : IRequest<Unit>;
