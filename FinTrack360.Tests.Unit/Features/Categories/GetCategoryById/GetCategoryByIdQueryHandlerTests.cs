using FinTrack360.Application.Features.Categories.GetCategoryById;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using FinTrack360.Tests.Unit.Mocks;

namespace FinTrack360.Tests.Unit.Features.Categories.GetCategoryById;

public class GetCategoryByIdQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly GetCategoryByIdQueryHandler _handler;
    private readonly Guid _categoryId;

    public GetCategoryByIdQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
        }, "mock"));

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        _categoryId = Guid.NewGuid();

        var categories = new List<Category>
        {
            new Category { Id = _categoryId, UserId = "test-user-id" },
            new Category { Id = Guid.NewGuid(), UserId = "other-user-id" }
        }.AsQueryable();

        var mockCategories = new Mock<DbSet<Category>>().SetupAsQueryable(categories);
        _dbContextMock.Setup(db => db.Categories).Returns(mockCategories.Object);

        _handler = new GetCategoryByIdQueryHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnCategory_WhenFound()
    {
        // Arrange
        var query = new GetCategoryByIdQuery(_categoryId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_categoryId, result.Id);
    }

    [Fact]
    public async Task Handle_Should_ReturnNull_WhenNotFound()
    {
        // Arrange
        var query = new GetCategoryByIdQuery(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
