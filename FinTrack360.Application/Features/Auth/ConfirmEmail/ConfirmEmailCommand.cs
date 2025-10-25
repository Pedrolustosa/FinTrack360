using MediatR;
namespace FinTrack360.Application.Features.Auth.ConfirmEmail;
public record ConfirmEmailCommand(string Email, string Token) : IRequest<Unit>;