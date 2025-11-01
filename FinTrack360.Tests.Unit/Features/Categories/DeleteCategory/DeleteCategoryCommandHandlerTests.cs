using FinTrack360.Application.Features.Categories.DeleteCategory;
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

namespace FinTrack360.Tests.Unit.Features.Categories.DeleteCategory;

public class DeleteCategoryCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly DeleteCategoryCommandHandler _handler;
    private readonly Guid _categoryId;

    public DeleteCategoryCommandHandlerTests()
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
            new Category { Id = _categoryId, UserId = "test-user-id" }
        }.AsQueryable();

        var mockCategories = new Mock<DbSet<Category>>().SetupAsQueryable(categories);
        _dbContextMock.Setup(db => db.Categories).Returns(mockCategories.Object);

        _handler = new DeleteCategoryCommandHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_DeleteCategory_WhenFound()
    {
        // Arrange
        var command = new DeleteCategoryCommand(_categoryId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContextMock.Verify(db => db.Categories.Remove(It.Is<Category>(c => c.Id == _categoryId)), Times.Once);
        _dbContextMock.Verify(db => db.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenNotFound()
    {
        // Arrange
        var command = new DeleteCategoryCommand(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
