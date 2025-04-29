using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Application.Abstractions.Services;
using HomeOwners.Application.DTOs.CommunityMeetings;
using HomeOwners.Application.ValidationErrors;
using HomeOwners.Domain.Models;
using Microsoft.Extensions.Logging;

namespace HomeOwners.Application.Services;

public class CommunityMeetingService : ICommunityMeetingService
{
    private readonly ICommunityMeetingRepository _repository;
    private readonly ILogger<CommunityMeetingService> _logger;

    public CommunityMeetingService(ICommunityMeetingRepository repository, ILoggerFactory loggerFactory)
    {
        _repository = repository;
        _logger = loggerFactory.CreateLogger<CommunityMeetingService>();
    }

    public async Task<long> CreateMeetingAsync(CreateCommunityMeetingDto model)
    {
        if (model.MeetingTime is null)
            throw new InvalidPropertyValueValidationError("Invalid meeting time");

        if (model.MeetingTime.Value < DateTime.UtcNow)
            throw new InvalidPropertyValueValidationError("Invalid meeting time");

        if (string.IsNullOrEmpty(model.Reason))
            throw new InvalidPropertyValueValidationError("Invalid meeting reason");

        if (model.Reason.Length > 512)
            throw new InvalidPropertyValueValidationError("Invalid meeting reason");

        var dbMeeting = new CommunityMeeting()
        {
            CommunityId = model.CommunityId,
            CreatorId = model.CreatorId,
            MeetingReason = model.Reason!,
            MeetingTime = model.MeetingTime.Value
        };

        await _repository.CreateAsync(dbMeeting);

        return dbMeeting.Id.Value;
    }

    public async Task<IEnumerable<CommunityMeetingDetailsDto>> GetForCommunityAsync(long communityId)
    {
        var dbMeetings = await _repository.GetAllByCommunityId(communityId);

        return dbMeetings.Select(x => new CommunityMeetingDetailsDto()
        {
            Reason = x.MeetingReason,
            MeetingTime = x.MeetingTime,
            CreateTimeStamp = x.CreateDate!.Value,
            CreatorUserName = x.Creator.Username!,
            LastUpdateTimeStamp = x.UpdateDate
        });
    }
}
