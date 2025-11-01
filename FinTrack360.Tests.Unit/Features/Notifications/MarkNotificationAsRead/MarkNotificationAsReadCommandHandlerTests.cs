using FinTrack360.Application.Features.Notifications.MarkNotificationAsRead;
using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FinTrack360.Tests.Unit.Mocks;
using Microsoft.EntityFrameworkCore;

namespace FinTrack360.Tests.Unit.Features.Notifications.MarkNotificationAsRead;

public class MarkNotificationAsReadCommandHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly MarkNotificationAsReadCommandHandler _handler;
    private readonly Guid _notificationId;

    public MarkNotificationAsReadCommandHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _notificationId = Guid.NewGuid();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "test-user-id") }, "mock"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var notifications = new List<Notification>
        {
            new Notification { Id = _notificationId, UserId = "test-user-id", IsRead = false }
        }.AsQueryable();

        var mockNotifications = new Mock<DbSet<Notification>>().SetupAsQueryable(notifications);
        _dbContextMock.Setup(db => db.Notifications).Returns(mockNotifications.Object);

        _handler = new MarkNotificationAsReadCommandHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_MarkNotificationAsRead_WhenFound()
    {
        // Arrange
        var command = new MarkNotificationAsReadCommand(_notificationId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var notification = _dbContextMock.Object.Notifications.First();
        Assert.True(notification.IsRead);
        _dbContextMock.Verify(db => db.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenNotFound()
    {
        // Arrange
        var command = new MarkNotificationAsReadCommand(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
}
