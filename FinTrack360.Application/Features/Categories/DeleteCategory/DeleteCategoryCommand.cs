using MediatR;

namespace FinTrack360.Application.Features.Categories.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest<Unit>;
