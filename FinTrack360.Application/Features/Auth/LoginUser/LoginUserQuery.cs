using MediatR;

namespace FinTrack360.Application.Features.Auth.LoginUser
{
    public record LoginUserQuery(string Email, string Password) : IRequest<string>;
}