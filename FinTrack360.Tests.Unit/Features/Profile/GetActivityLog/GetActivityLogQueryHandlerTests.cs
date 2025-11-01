using FinTrack360.Application.Features.Profile.GetActivityLog;
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

namespace FinTrack360.Tests.Unit.Features.Profile.GetActivityLog;

public class GetActivityLogQueryHandlerTests
{
    private readonly Mock<IAppDbContext> _dbContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly GetActivityLogQueryHandler _handler;

    public GetActivityLogQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAppDbContext>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "test-user-id") }, "mock"));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });

        var logs = new List<ActivityLog>
        {
            new ActivityLog { UserId = "test-user-id", ActionType = "Log 2", Timestamp = DateTime.UtcNow.AddMinutes(-5) },
            new ActivityLog { UserId = "test-user-id", ActionType = "Log 1", Timestamp = DateTime.UtcNow.AddMinutes(-10) },
            new ActivityLog { UserId = "other-user-id", ActionType = "Other Log", Timestamp = DateTime.UtcNow },
        }.AsQueryable();

        var mockLogs = new Mock<DbSet<ActivityLog>>().SetupAsQueryable(logs);
        _dbContextMock.Setup(db => db.ActivityLogs).Returns(mockLogs.Object);

        _handler = new GetActivityLogQueryHandler(_dbContextMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnUserActivityLogs_OrderedByTimestampDesc()
    {
        // Arrange
        var query = new GetActivityLogQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal("Log 2", resultList[0].ActionType);
        Assert.Equal("Log 1", resultList[1].ActionType);
    }
}
