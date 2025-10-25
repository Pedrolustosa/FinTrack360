using FinTrack360.Domain.Entities;
namespace FinTrack360.Application.Common.Interfaces;
public interface IJwtTokenGenerator
{
    string GenerateToken(ApplicationUser user);
}