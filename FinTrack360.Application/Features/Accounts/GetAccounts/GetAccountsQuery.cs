using FinTrack360.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace FinTrack360.Application.Features.Accounts.GetAccounts;

public record GetAccountsQuery : IRequest<IEnumerable<Account>>;
