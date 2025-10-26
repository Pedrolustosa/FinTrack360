using FinTrack360.Domain.Entities;
using MediatR;

namespace FinTrack360.Application.Features.Transactions.GetTransactionById;

public record GetTransactionByIdQuery(Guid AccountId, Guid Id) : IRequest<Transaction?>;
