using MediatR;

namespace FinTrack360.Application.Features.Auth.ForgotPassword;

public record ForgotPasswordCommand(string Email, string Language) : IRequest<Unit>;