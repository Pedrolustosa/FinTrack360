using MediatR;

namespace FinTrack360.Application.Features.Auth.RegisterUser;
public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Language) : IRequest<string>;