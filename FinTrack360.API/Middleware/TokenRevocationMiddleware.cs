using FinTrack360.Application.Common.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace FinTrack360.API.Middleware;

public class TokenRevocationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITokenRevocationService tokenRevocationService)
    {
        var jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

        if (jti is not null && await tokenRevocationService.IsTokenRevokedAsync(jti))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("This token has been revoked.");
            return;
        }

        await next(context);
    }
}
