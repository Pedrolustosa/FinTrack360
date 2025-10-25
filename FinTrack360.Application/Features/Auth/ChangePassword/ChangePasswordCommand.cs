using FinTrack360.Application.Common.Interfaces;
using MediatR;

namespace FinTrack360.Application.Features.Auth.ChangePassword;

public record ChangePasswordCommand(string OldPassword, string NewPassword, string ConfirmPassword, string UserId) : IRequest<Unit>, IAuditableRequest;
