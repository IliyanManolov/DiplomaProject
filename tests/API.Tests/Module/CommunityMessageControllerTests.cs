using API.Tests.Fixtures;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.CommunityMessages;
using HomeOwners.Web.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace API.Tests.Module;

[Collection("ControllerCollection")]
public class CommunityMessageControllerTests : BaseControllerTest
{
    private readonly InMemoryFixture _fixture;
    private readonly CommunityMessagesController _controller;

    public CommunityMessageControllerTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
        _controller = new CommunityMessagesController(
            communitySerivce: _fixture.ServiceProvider.GetRequiredService<ICommunityService>(),
            userService: _fixture.ServiceProvider.GetRequiredService<IUserService>(),
            messagesService: _fixture.ServiceProvider.GetRequiredService<ICommunityMessagesService>(),
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

        var result = AssertOk<List<CommunityMessageDetailsDto>>(actionResult);

        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task ShouldHandleNoMessages()
    {
        var actionResult = await _controller.GetForCommunityAsync(4);

        var result = AssertOk<List<CommunityMessageDetailsDto>>(actionResult);

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
        var actionResult = await _controller.GetMessageByIdAsync(1);

        var result = AssertOk<CommunityMessageDetailsDto>(actionResult);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task ShouldHandleNonExistingMeeting()
    {
        var actionResult = await _controller.GetMessageByIdAsync(123456);

        var errors = AssertBadRequest(actionResult);

        Assert.NotEmpty(errors);
        Assert.Single(errors);
        Assert.Equal("InvalidPropertyValueValidationError", errors[0].Name);
        Assert.Contains("Invalid message id", errors[0].Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public async Task ShouldNotAllowNormalUsersToCreate()
    {
        var dto = new CreateCommunityMessageDto()
        {
            CommunityId = 1,
            CreatorId = 2,
            Message = "Random message"
        };

        var actionResult = await _controller.CreateMessageAsync(dto);

        AssertNotFound(actionResult);
    }
}
