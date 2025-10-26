using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.Accounts.GetAccountById;

public class GetAccountByIdQueryHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetAccountByIdQuery, Account?>
{
    public async Task<Account?> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        return await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.UserId == userId, cancellationToken);
    }
}
