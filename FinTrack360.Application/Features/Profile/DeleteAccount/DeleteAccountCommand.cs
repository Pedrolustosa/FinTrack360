using MediatR;

namespace FinTrack360.Application.Features.Profile.DeleteAccount;

public record DeleteAccountCommand(string Password) : IRequest<Unit>;
