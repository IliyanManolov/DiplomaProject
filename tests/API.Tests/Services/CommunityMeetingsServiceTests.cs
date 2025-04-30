using API.Tests.Fixtures;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.CommunityMeetings;
using HomeOwners.Application.DTOs.User;
using HomeOwners.Application.ValidationErrors;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;

namespace API.Tests.Services;

[Collection("BasicCollection")]
public class CommunityMeetingsServiceTests
{
    private readonly InMemoryFixture _fixture;
    private readonly ICommunityMeetingService _service;
    private const long _basicCommunityId = 1;
    private const long _adminId = 1;

    public CommunityMeetingsServiceTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
        _service = _fixture.ServiceProvider.GetRequiredService<ICommunityMeetingService>();
    }

    [Fact]
    public async Task ShouldReturnAll()
    {
        var result = await _service.GetForCommunityAsync(_basicCommunityId);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.True(result.Count() > 1);

        Assert.True(result.All(x => !string.IsNullOrEmpty(x.Reason)));
        Assert.True(result.All(x => !string.IsNullOrEmpty(x.CreatorUserName)));
    }

    [Fact]
    public async Task ShouldCreate()
    {
        var meetingTime = DateTime.UtcNow.AddDays(2);
        var reason = "Create Test 01";
        var dto = new CreateCommunityMeetingDto()
        {
            CommunityId = _basicCommunityId,
            CreatorId = _adminId,
            MeetingTime = meetingTime,
            Reason = reason
        };

        var id = await _service.CreateMeetingAsync(dto);

        var allMeetings = await _service.GetForCommunityAsync(_basicCommunityId);

        var target = allMeetings.FirstOrDefault(x => x.Reason == reason);
        Assert.NotNull(target);
    }

    public static List<object[]> ShouldFailToCreate_Data => new()
    {
        new object[]
        {
            new CreateCommunityMeetingDto()
            {
                CommunityId = _basicCommunityId,
                CreatorId = _adminId,
                MeetingTime = DateTime.UtcNow.AddDays(-1),
                Reason = "Invalid Test Create 01"
            },
            typeof(InvalidPropertyValueValidationError),
            "time"
        },
        new object[]
        {
            new CreateCommunityMeetingDto()
            {
                CommunityId = _basicCommunityId,
                CreatorId = _adminId,
                MeetingTime = null,
                Reason = "Invalid Test Create 02"
            },
            typeof(InvalidPropertyValueValidationError),
            "time"
        },
        new object[]
        {
            new CreateCommunityMeetingDto()
            {
                CommunityId = _basicCommunityId,
                CreatorId = _adminId,
                MeetingTime = DateTime.UtcNow.AddDays(2),
                Reason = ""
            },
            typeof(InvalidPropertyValueValidationError),
            "reason"
        },
        new object[]
        {
            new CreateCommunityMeetingDto()
            {
                CommunityId = _basicCommunityId,
                CreatorId = _adminId,
                MeetingTime = DateTime.UtcNow.AddDays(2),
                Reason = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent placerat mauris non vehicula ultrices. Donec rutrum suscipit purus at rutrum. Proin convallis odio eget mi porttitor, id tincidunt justo tincidunt. In quis dui tincidunt, bibendum magna in, vehicula augue. Duis in quam at sapien elementum tempor. Sed vestibulum, turpis in consectetur facilisis, odio sapien rutrum lorem, eu fringilla mauris ante vitae massa. Phasellus tempor commodo justo, non tempor mi mattis id. Integer sagittis finibus volutpat. In hac habitasse platea dictumst. Phasellus elementum lectus vitae arcu pulvinar fringilla. Nulla ultrices ipsum vitae dui vestibulum porta. Praesent cursus lectus in nisi varius."
            },
            typeof(InvalidPropertyValueValidationError),
            "reason"
        }
    };

    [Theory]
    [MemberData(nameof(ShouldFailToCreate_Data))]
    public async Task ShouldFailToCreate(CreateCommunityMeetingDto model, Type validationErrorType, string messageContains)
    {
        var ex = await Assert.ThrowsAsync(validationErrorType, async () => await _service.CreateMeetingAsync(model));

        Assert.Contains(messageContains, ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }
}