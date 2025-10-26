using FinTrack360.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace FinTrack360.Application.Features.Categories.GetCategories;

public record GetCategoriesQuery : IRequest<IEnumerable<Category>>;
