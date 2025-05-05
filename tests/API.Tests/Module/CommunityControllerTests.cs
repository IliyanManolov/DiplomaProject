using API.Tests.Fixtures;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.Community;
using HomeOwners.Web.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace API.Tests.Module;

[Collection("ControllerCollection")]
public class CommunityControllerTests : BaseControllerTest
{
    private readonly InMemoryFixture _fixture;
    private readonly CommunitiesController _controller;

    public CommunityControllerTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
        _controller = new CommunitiesController(
            communitySerivce: _fixture.ServiceProvider.GetRequiredService<ICommunityService>(),
            userService: _fixture.ServiceProvider.GetRequiredService<IUserService>(),
            calculationService: _fixture.ServiceProvider.GetRequiredService<IDuesCalculationService>(),
            loggerFactory: new NullLoggerFactory());

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task ShouldCalculateDues()
    {
        var dto = new CalculateDuesDto()
        {
            UserId = _adminId
        };

        var actionResult = await _controller.UpdateDuesAsync(dto);

        var result = AssertOk<bool>(actionResult);

        Assert.True(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(_deletedUserId)]
    [InlineData((long)2)] // Normal user, NOT admin
    [InlineData((long)123456)] // Nonexisting user
    public async Task ShouldFailToCalculateDues(long? userId)
    {
        var dto = new CalculateDuesDto()
        {
            UserId = userId
        };

        var actionResult = await _controller.UpdateDuesAsync(dto);

        AssertNotFound(actionResult);
    }

    [Theory]
    [InlineData(_deletedUserId)]
    //[InlineData((long)2)] // Normal user, NOT admin
    [InlineData((long)123456)] // Nonexisting user
    public async Task ShouldFailToGetUserCommunities(long userId)
    {
        var actionResult = await _controller.GetCommunitiesAsync(userId);

        AssertNotFound(actionResult);
    }

    // User has no property but his referral code is related to a community
    [Fact]
    public async Task ShouldReturnSingleCommunity()
    {
        var actionResult = await _controller.GetCommunitiesAsync(5);

        var result = AssertOk<List<CommunityDetailsDto>>(actionResult);

        Assert.Single(result.ToList());
    }

    // Admins should have all communities returned
    [Fact]
    public async Task ShouldReturnAllCommunitiesForAdmin()
    {
        var actionResult = await _controller.GetCommunitiesAsync(_adminId);

        var result = AssertOk<List<CommunityDetailsDto>>(actionResult);

        // Avoid hardcoding a value.
        Assert.True(result.ToList().Count > 3);
    }

    [Fact]
    public async Task ShouldFailToCreateCommunity()
    {
        var dto = new CreateCommunityDto()
        {
            Name = ""
        };

        var actionResult = await _controller.CreateCommunityAsync(dto);

        var errors = AssertBadRequest(actionResult);

        Assert.NotEmpty(errors);
    }
}