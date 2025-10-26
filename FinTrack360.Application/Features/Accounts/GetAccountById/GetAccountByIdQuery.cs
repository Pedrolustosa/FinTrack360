using FinTrack360.Domain.Entities;
using MediatR;

namespace FinTrack360.Application.Features.Accounts.GetAccountById;

public record GetAccountByIdQuery(Guid Id) : IRequest<Account?>;
