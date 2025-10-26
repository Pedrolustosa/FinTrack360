using FinTrack360.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.Accounts.UpdateAccount;

public class UpdateAccountCommandHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<UpdateAccountCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var account = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.UserId == userId, cancellationToken) ?? throw new Exception("Account not found.");

        account.Name = request.Name;
        account.Type = request.Type;
        account.InstitutionName = request.InstitutionName;
        account.CurrentBalance = request.CurrentBalance;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
