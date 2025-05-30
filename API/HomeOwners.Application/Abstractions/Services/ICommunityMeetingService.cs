﻿using HomeOwners.Application.DTOs.CommunityMeetings;

namespace HomeOwners.Application.Abstractions.Services;

public interface ICommunityMeetingService
{
    public Task<long> CreateMeetingAsync(CreateCommunityMeetingDto model);
    public Task<IEnumerable<CommunityMeetingDetailsDto>> GetForCommunityAsync(long communityId);
    public Task<CommunityMeetingDetailsDto> GetByIdAsync(long? id);
    public Task<CommunityMeetingDetailsDto> UpdateMeeting(EditCommunityMeetingDto model);
    public Task<CommunityMeetingDetailsDto> DeleteByIdAsync(long? id);
}