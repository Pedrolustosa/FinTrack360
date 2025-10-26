using FinTrack360.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.Categories.UpdateCategory;

public class UpdateCategoryCommandHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<UpdateCategoryCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var category = await dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == userId, cancellationToken) ?? throw new Exception("Category not found.");

        category.Name = request.Name;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
