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

    public async Task<CommunityMeetingDetailsDto> DeleteByIdAsync(long? id)
    {
        if (id == null)
            throw new InvalidPropertyValueValidationError("Invalid meeting id");

        var dbMeeting = await _repository.GetByIdAsync(id);

        if (dbMeeting == null)
        {
            _logger.LogInformation("Meeting with ID '{id}' not found ", id);
            throw new InvalidPropertyValueValidationError("Invalid meeting id");
        }

        var response = new CommunityMeetingDetailsDto()
        {
            Id = dbMeeting.Id!.Value,
            CommunityId = dbMeeting.CommunityId!.Value,
            Reason = dbMeeting.MeetingReason,
            MeetingTime = dbMeeting.MeetingTime,
            CreateTimeStamp = dbMeeting.CreateDate!.Value,
            CreatorUserName = dbMeeting.Creator.Username!,
            LastUpdateTimeStamp = dbMeeting.UpdateDate
        };

        await _repository.DeleteAsync(dbMeeting);

        return response;
    }

    public async Task<CommunityMeetingDetailsDto> GetByIdAsync(long? id)
    {
        if (id == null)
            throw new InvalidPropertyValueValidationError("Invalid meeting id");

        var dbMeeting = await _repository.GetByIdAsync(id);

        if (dbMeeting == null)
        {
            _logger.LogInformation("Meeting with ID '{id}' not found ", id);
            throw new InvalidPropertyValueValidationError("Invalid meeting id");
        }

        return new CommunityMeetingDetailsDto()
        {
            Id = dbMeeting.Id!.Value,
            CommunityId = dbMeeting.CommunityId!.Value,
            Reason = dbMeeting.MeetingReason,
            MeetingTime = dbMeeting.MeetingTime,
            CreateTimeStamp = dbMeeting.CreateDate!.Value,
            CreatorUserName = dbMeeting.Creator.Username!,
            LastUpdateTimeStamp = dbMeeting.UpdateDate
        };
    }

    public async Task<IEnumerable<CommunityMeetingDetailsDto>> GetForCommunityAsync(long communityId)
    {
        var dbMeetings = await _repository.GetAllByCommunityId(communityId);

        return dbMeetings.Select(x => new CommunityMeetingDetailsDto()
        {
            Id = x.Id!.Value,
            CommunityId = x.CommunityId!.Value,
            Reason = x.MeetingReason,
            MeetingTime = x.MeetingTime,
            CreateTimeStamp = x.CreateDate!.Value,
            CreatorUserName = x.Creator.Username!,
            LastUpdateTimeStamp = x.UpdateDate
        });
    }

    public async Task<CommunityMeetingDetailsDto> UpdateMeeting(EditCommunityMeetingDto model)
    {
        if (model.MeetingTime is null)
            throw new InvalidPropertyValueValidationError("Invalid meeting time");

        if (model.MeetingTime.Value < DateTime.UtcNow)
            throw new InvalidPropertyValueValidationError("Invalid meeting time");

        if (string.IsNullOrEmpty(model.NewReason))
            throw new InvalidPropertyValueValidationError("Invalid meeting reason");

        if (model.NewReason.Length > 512)
            throw new InvalidPropertyValueValidationError("Invalid meeting reason");

        if (model.Id == null)
            throw new InvalidPropertyValueValidationError("Invalid id");

        var dbMeeting = await _repository.GetByIdAsync(model.Id);

        if (dbMeeting == null)
        {
            _logger.LogInformation("Meeting with ID '{id}' not found ", model.Id);
            throw new InvalidPropertyValueValidationError("Invalid meeting id");
        }

        dbMeeting.MeetingTime = model.MeetingTime!.Value;
        dbMeeting.MeetingReason = model.NewReason;

        await _repository.UpdateAsync(dbMeeting);

        return new CommunityMeetingDetailsDto()
        {
            Id = dbMeeting.Id!.Value,
            CommunityId = dbMeeting.CommunityId!.Value,
            Reason = dbMeeting.MeetingReason,
            MeetingTime = dbMeeting.MeetingTime,
            CreateTimeStamp = dbMeeting.CreateDate!.Value,
            CreatorUserName = dbMeeting.Creator.Username!,
            LastUpdateTimeStamp = dbMeeting.UpdateDate
        };
    }
}
