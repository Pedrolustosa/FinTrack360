using MediatR;
using Microsoft.AspNetCore.Http;

namespace FinTrack360.Application.Features.Transactions.ImportTransactions;

public record ImportTransactionsCommand(Guid AccountId, IFormFile File) : IRequest<Unit>;
