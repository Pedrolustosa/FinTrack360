using MediatR;

namespace FinTrack360.Application.Features.Accounts.DeleteAccount;

public record DeleteAccountCommand(Guid Id) : IRequest<Unit>;
