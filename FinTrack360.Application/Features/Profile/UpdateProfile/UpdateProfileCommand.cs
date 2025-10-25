using MediatR;

namespace FinTrack360.Application.Features.Profile.UpdateProfile;

public record UpdateProfileCommand(string FirstName, string LastName, string? PhoneNumber) : IRequest<Unit>;
