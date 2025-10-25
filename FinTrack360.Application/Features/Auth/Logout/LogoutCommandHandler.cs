using FinTrack360.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace FinTrack360.Application.Features.Auth.Logout;

public class LogoutCommandHandler(ITokenRevocationService tokenRevocationService, IHttpContextAccessor httpContextAccessor) : IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var jti = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        var exp = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;

        if (jti is not null && long.TryParse(exp, out var expSeconds))
        {
            var expiration = DateTimeOffset.FromUnixTimeSeconds(expSeconds).DateTime;
            await tokenRevocationService.RevokeTokenAsync(jti, expiration);
        }

        return Unit.Value;
    }
}
