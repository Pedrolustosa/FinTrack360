using FinTrack360.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.Accounts.DeleteAccount;

public class DeleteAccountCommandHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<DeleteAccountCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var account = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.UserId == userId, cancellationToken) ?? throw new Exception("Account not found.");

        dbContext.Accounts.Remove(account);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
