using API.Tests.Fixtures;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.CommunityMessages;
using HomeOwners.Application.ValidationErrors;
using Microsoft.Extensions.DependencyInjection;

namespace API.Tests.Services;

[Collection("BasicCollection")]
public class CommunityMessageServiceTests
{
    private readonly InMemoryFixture _fixture;
    private readonly ICommunityMessagesService _service;
    private const long _basicCommunityId = 1;
    private const long _adminId = 1;

    public CommunityMessageServiceTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
        _service = _fixture.ServiceProvider.GetRequiredService<ICommunityMessagesService>();
    }

    [Fact]
    public async Task ShouldReturnAll()
    {
        var result = await _service.GetForCommunityAsync(_basicCommunityId);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.True(result.Count() > 1);

        Assert.True(result.All(x => !string.IsNullOrEmpty(x.Message)));
        Assert.True(result.All(x => !string.IsNullOrEmpty(x.CreatorUserName)));
    }

    [Fact]
    public async Task ShouldCreate()
    {
        var message = "Create Test 01";
        var dto = new CreateCommunityMessageDto()
        {
            CommunityId = _basicCommunityId,
            CreatorId = _adminId,
            Message = message
        };

        var id = await _service.CreateMessageAsync(dto);

        var allMessages = await _service.GetForCommunityAsync(_basicCommunityId);

        var target = allMessages.FirstOrDefault(x => x.Message == message);

        Assert.NotNull(target);
        Assert.False(string.IsNullOrEmpty(target.CreatorUserName));
    }

    [Fact]
    public async Task ShouldGetById()
    {
        var response = await _service.GetByIdAsync(1);

        Assert.NotNull(response);
        Assert.Equal(1, response.Id);
        Assert.False(string.IsNullOrEmpty(response.CreatorUserName));
        Assert.False(string.IsNullOrEmpty(response.Message));
    }

    [Theory]
    [InlineData(null, "empty")]
    [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent placerat mauris non vehicula ultric" +
        "es. Donec rutrum suscipit purus at rutrum. Proin convallis odio eget mi porttitor, id tincidunt justo tincidunt. " +
        "In quis dui tincidunt, bibendum magna in, vehicula augue. Duis in quam at sapien elementum tempor. Sed vestibulum, " +
        "turpis in consectetur facilisis, odio sapien rutrum lorem, eu fringilla mauris ante vitae massa. Phasellus tempor c" +
        "ommodo justo, non tempor mi mattis id. Integer sagittis finibus volutpat. In hac habitasse platea dictumst. Phasellus " +
        "elementum lectus vitae arcu pulvinar fringilla. Nulla ultrices ipsum vitae dui vestibulum porta. Praesent cursus lectus in nisi varius.", 
        "invalid")]
    public async Task ShouldFailToCreate(string message, string messageContains)
    {

        var model = new CreateCommunityMessageDto()
        {
            CommunityId = _basicCommunityId,
            CreatorId = _adminId,
            Message = message
        };

        var ex = await Assert.ThrowsAsync<InvalidPropertyValueValidationError>(async () => await _service.CreateMessageAsync(model));

        Assert.Contains(messageContains, ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }
}