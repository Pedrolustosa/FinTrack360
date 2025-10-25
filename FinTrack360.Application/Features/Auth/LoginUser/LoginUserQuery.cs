using FinTrack360.Application.Common.Interfaces;
using MediatR;

namespace FinTrack360.Application.Features.Auth.LoginUser
{
    public record LoginUserQuery(string Email, string Password) : IRequest<string>, IAuditableRequest
    {
        public string UserId => Email;
    }
}