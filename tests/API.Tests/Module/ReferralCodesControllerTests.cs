using API.Tests.Fixtures;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.ReferralCodes;
using HomeOwners.Domain.Models;
using HomeOwners.Web.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace API.Tests.Module;

[Collection("ControllerCollection")]
public class ReferralCodesControllerTests : BaseControllerTest
{
    private readonly InMemoryFixture _fixture;
    private readonly ReferralCodesController _controller;

    public ReferralCodesControllerTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
        _controller = new ReferralCodesController(
            communitySerivce: _fixture.ServiceProvider.GetRequiredService<ICommunityService>(),
            userService: _fixture.ServiceProvider.GetRequiredService<IUserService>(),
            codeService: _fixture.ServiceProvider.GetRequiredService<IReferralCodeService>(),
            loggerFactory: new NullLoggerFactory());

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Theory]
    [InlineData(2)] // Normal user
    [InlineData(123456)] // Nonexisting user
    public async Task ShouldHandleUnAuthorizedUser(long userId)
    {
        var dto = new CreateReferralCodesDto()
        {
            CommunityId = 1,
            Count = 1,
            CreatorId = userId,
        };

        var actionResult = await _controller.CreateBulkAsync(dto);

        AssertNotFound(actionResult);
    }

    [Theory]
    [InlineData(123456)]
    public async Task ShouldHandleInvalidCommunityId(long communityId)
    {
        var dto = new CreateReferralCodesDto()
        {
            CommunityId = communityId,
            Count = 1,
            CreatorId = _adminId,
        };

        var actionResult = await _controller.CreateBulkAsync(dto);

        AssertNotFound(actionResult);
    }

    [Fact]
    public async Task ShouldCreateCodes()
    {
        int codesCount = 3;
        var dto = new CreateReferralCodesDto()
        {
            CommunityId = 1,
            Count = codesCount,
            CreatorId = _adminId,
        };

        var actionResult = await _controller.CreateBulkAsync(dto);

        var result = AssertOk<HashSet<string>>(actionResult);

        Assert.NotEmpty(result);
        Assert.Equal(codesCount, result.Count);
        Assert.True(result.All(x => Guid.TryParse(x, out _)), "Not all codes returned are valid GUID");
    }
}