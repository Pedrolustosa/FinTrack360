using MediatR;

namespace FinTrack360.Application.Features.Auth.RetryConfirmationEmail;

public record RetryConfirmationEmailCommand(string Email, string Language) : IRequest<Unit>;
