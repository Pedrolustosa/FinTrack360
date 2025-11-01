using FinTrack360.Application.Features.Notifications.GetNotifications;
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

namespace FinTrack360.Tests.Unit.Features.Notifications.GetNotifications;

public class GetNotificationsQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly GetNotificationsQueryHandler _handler;

    public GetNotificationsQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "test-user-id") }, "mock"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var notifications = new List<Notification>
        {
            new Notification { UserId = "test-user-id", Message = "Unread 1", IsRead = false, Timestamp = DateTime.UtcNow.AddMinutes(-10) },
            new Notification { UserId = "test-user-id", Message = "Unread 2", IsRead = false, Timestamp = DateTime.UtcNow.AddMinutes(-5) },
            new Notification { UserId = "test-user-id", Message = "Read 1", IsRead = true, Timestamp = DateTime.UtcNow.AddMinutes(-20) },
            new Notification { UserId = "other-user-id", Message = "Other User Unread", IsRead = false, Timestamp = DateTime.UtcNow.AddMinutes(-1) },
        }.AsQueryable();

        var mockNotifications = new Mock<DbSet<Notification>>().SetupAsQueryable(notifications);
        _dbContextMock.Setup(db => db.Notifications).Returns(mockNotifications.Object);

        _handler = new GetNotificationsQueryHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnUnreadNotificationsForUser_OrderedByTimestampDesc()
    {
        // Arrange
        var query = new GetNotificationsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal("Unread 2", resultList[0].Message);
        Assert.Equal("Unread 1", resultList[1].Message);
    }
}
