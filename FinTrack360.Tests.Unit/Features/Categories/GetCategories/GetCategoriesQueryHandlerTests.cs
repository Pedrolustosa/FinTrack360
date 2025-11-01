using FinTrack360.Application.Features.Categories.GetCategories;
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

namespace FinTrack360.Tests.Unit.Features.Categories.GetCategories;

public class GetCategoriesQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly GetCategoriesQueryHandler _handler;

    public GetCategoriesQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
        }, "mock"));

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var categories = new List<Category>
        {
            new Category { UserId = "test-user-id", Name = "B" },
            new Category { UserId = "test-user-id", Name = "A" },
            new Category { UserId = "other-user-id", Name = "C" }
        }.AsQueryable();

        var mockCategories = new Mock<DbSet<Category>>().SetupAsQueryable(categories);
        _dbContextMock.Setup(db => db.Categories).Returns(mockCategories.Object);

        _handler = new GetCategoriesQueryHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnUserCategories_OrderedByName()
    {
        // Arrange
        var query = new GetCategoriesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal("A", result.First().Name);
    }
}
