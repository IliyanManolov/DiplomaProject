using API.Tests.Fixtures;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.CommunityMeetings;
using HomeOwners.Web.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace API.Tests.Controllers;

[Collection("ControllerCollection")]
public class CommunityMeetingControllerTests : BaseControllerTest
{
    private readonly InMemoryFixture _fixture;
    private readonly CommunityMeetingsController _controller;

    public CommunityMeetingControllerTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
        _controller = new CommunityMeetingsController(
            communitySerivce: _fixture.ServiceProvider.GetRequiredService<ICommunityService>(),
            userService: _fixture.ServiceProvider.GetRequiredService<IUserService>(),
            meetingsService: _fixture.ServiceProvider.GetRequiredService<ICommunityMeetingService>(),
            loggerFactory: new NullLoggerFactory());

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task ShouldGetForCommunity()
    {
        var actionResult = await _controller.GetForCommunityAsync(1);

        var result = AssertOk<List<CommunityMeetingDetailsDto>>(actionResult);

        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task ShouldHandleNoMessages()
    {
        var actionResult = await _controller.GetForCommunityAsync(4);

        var result = AssertOk<List<CommunityMeetingDetailsDto>>(actionResult);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ShouldHandleNonExistingCommunity()
    {
        var actionResult = await _controller.GetForCommunityAsync(123456);

        AssertNotFound(actionResult);
    }

    [Fact]
    public async Task ShouldGetById()
    {
        var actionResult = await _controller.GetMeetingByIdAsync(1);

        var result = AssertOk<CommunityMeetingDetailsDto>(actionResult);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task ShouldHandleNonExistingMeeting()
    {
        var actionResult = await _controller.GetMeetingByIdAsync(123456);

        var errors = AssertBadRequest(actionResult);

        Assert.NotEmpty(errors);
        Assert.Single(errors);
        Assert.Equal("InvalidPropertyValueValidationError", errors[0].Name);
        Assert.Contains("Invalid meeting id", errors[0].Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public async Task ShouldNotAllowNormalUsersToCreate()
    {
        var dto = new CreateCommunityMeetingDto()
        {
            CommunityId = 1,
            CreatorId = 2,
            MeetingTime = DateTime.UtcNow.AddDays(2),
            Reason = "Random Reason"
        };

        var actionResult = await _controller.CreateMeetingAsync(dto);

        AssertNotFound(actionResult);
    }
}
