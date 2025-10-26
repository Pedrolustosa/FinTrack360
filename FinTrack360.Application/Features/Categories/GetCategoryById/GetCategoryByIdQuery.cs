using FinTrack360.Domain.Entities;
using MediatR;

namespace FinTrack360.Application.Features.Categories.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<Category?>;
