using FinTrack360.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FinTrack360.Application.Features.Categories.DeleteCategory;

public class DeleteCategoryCommandHandler(IAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<DeleteCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var category = await dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == userId, cancellationToken) ?? throw new Exception("Category not found.");

        // Optional: Decide what to do with transactions associated with this category. 
        // For now, we'll just delete the category.

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
