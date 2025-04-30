using API.Tests.Fixtures;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.Community;
using HomeOwners.Application.ValidationErrors;
using HomeOwners.Application.ValidationErrors.Authentication;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;

namespace API.Tests.Services;

[Collection("BasicCollection")]
public class CommunityServiceTests
{
    private readonly InMemoryFixture _fixture;
    private readonly ICommunityService _service;
    private const long _adminId = 1;

    public CommunityServiceTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
        _service = _fixture.ServiceProvider.GetRequiredService<ICommunityService>();
    }

    [Fact]
    public async Task ShouldCreateCommunity()
    {
        var dto = new CreateCommunityDto()
        {
            Name = "TestCreate01"
        };

        var id = await _service.CreateCommunityAsync(dto);

        var details = await _service.GetCommunityDetailsAsync(id);

        Assert.NotNull(details);
        Assert.Equal(dto.Name, details.Name);
        Assert.Equal(0, details.PropertiesCount);
    }

    [Fact]
    public async Task ShouldHandleDuplicateNames()
    {
        var dto = new CreateCommunityDto()
        {
            Name = "DuplicatedName"
        };

        await _service.CreateCommunityAsync(dto);

        var ex = await Assert.ThrowsAsync<InvalidPropertyValueValidationError>(async () => await _service.CreateCommunityAsync(dto));
        Assert.Contains("exists", ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task ShouldHandleInvalidName(string? communityName)
    {
        var dto = new CreateCommunityDto()
        {
            Name = communityName
        };

        var ex = await Assert.ThrowsAsync<InvalidPropertyValueValidationError>(async () => await _service.CreateCommunityAsync(dto));
        Assert.Contains("invalid", ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Theory]
    [InlineData(null, "null")]
    [InlineData((long)123456, "does not exist")]
    public async Task ShouldHandleInvalidCommunityId(long? communityId, string errorContains)
    {
        var ex = await Assert.ThrowsAsync<CommunityAuthenticationValidationError>(async () => await _service.GetCommunityDetailsAsync(communityId));
        Assert.Contains(errorContains, ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public async Task ShouldGetAllCommunities()
    {
        var result = await _service.GetAllCommunities();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.True(result.Count() > 1);
    }
}
