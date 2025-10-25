using FinTrack360.Domain.Enums;
using MediatR;

namespace FinTrack360.Application.Features.Accounts.UpdateAccount;

public record UpdateAccountCommand(
    Guid Id,
    string Name,
    AccountType Type,
    string InstitutionName,
    decimal CurrentBalance) : IRequest<Unit>;
