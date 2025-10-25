namespace FinTrack360.Application.Common.Interfaces;

public interface ITokenRevocationService
{
    Task RevokeTokenAsync(string jti, DateTime expiration);
    Task<bool> IsTokenRevokedAsync(string jti);
}
