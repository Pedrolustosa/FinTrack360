using MediatR;

namespace FinTrack360.Application.Features.Auth.Logout;

public record LogoutCommand : IRequest<Unit>;
