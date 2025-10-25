using FinTrack360.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace FinTrack360.Infrastructure.Services;

public class TokenRevocationService(IMemoryCache memoryCache) : ITokenRevocationService
{
    public Task RevokeTokenAsync(string jti, DateTime expiration)
    {
        var expirationTimeSpan = expiration.ToUniversalTime() - DateTime.UtcNow;
        if (expirationTimeSpan > TimeSpan.Zero)
        {
            memoryCache.Set(jti, string.Empty, expirationTimeSpan);
        }

        return Task.CompletedTask;
    }

    public Task<bool> IsTokenRevokedAsync(string jti)
    {
        return Task.FromResult(memoryCache.TryGetValue(jti, out _));
    }
}
