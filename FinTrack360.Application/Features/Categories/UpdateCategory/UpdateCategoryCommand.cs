using MediatR;

namespace FinTrack360.Application.Features.Categories.UpdateCategory;

public record UpdateCategoryCommand(Guid Id, string Name) : IRequest<Unit>;
