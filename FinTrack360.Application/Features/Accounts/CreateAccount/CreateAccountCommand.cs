using FinTrack360.Domain.Enums;
using MediatR;

namespace FinTrack360.Application.Features.Accounts.CreateAccount;

public record CreateAccountCommand(
    string Name,
    AccountType Type,
    string InstitutionName,
    decimal InitialBalance) : IRequest<Guid>;
