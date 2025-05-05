using API.Tests.Fixtures;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.Property;
using HomeOwners.Web.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace API.Tests.Module;

[Collection("ControllerCollection")]
public class PropertiesControllerTests : BaseControllerTest
{
    private readonly InMemoryFixture _fixture;
    private readonly PropertiesController _controller;

    public PropertiesControllerTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
        _controller = new PropertiesController(propertiesService: _fixture.ServiceProvider.GetRequiredService<IPropertyService>(),
            communitySerivce: _fixture.ServiceProvider.GetRequiredService<ICommunityService>(),
            userService: _fixture.ServiceProvider.GetRequiredService<IUserService>(),
            loggerFactory: new NullLoggerFactory());

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Theory]
    [InlineData(_deletedUserId, (long)1)]
    [InlineData(null, (long)1)]
    [InlineData(_adminId, null)]
    [InlineData((long)2, (long)1)] // User has no properties in the community
    public async Task ShouldFailToGetProperties(long? userId, long? communityId)
    {
        var dto = new GetPropertyShortDto()
        {
            CommunityId = communityId,
            UserId = userId
        };

        var actionResult = await _controller.GetPropertiesAsync(dto);

        AssertNotFound(actionResult);
    }
}