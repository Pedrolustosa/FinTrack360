using MediatR;

namespace FinTrack360.Application.Features.Categories.CreateCategory;

public record CreateCategoryCommand(string Name) : IRequest<Guid>;
