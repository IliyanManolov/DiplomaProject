using HomeOwners.Application.DTOs.CommunityMessages;

namespace HomeOwners.Application.Abstractions.Services;

public interface ICommunityMessagesService
{
    public Task<long> CreateMessageAsync(CreateCommunityMessageDto model);
    public Task<IEnumerable<CommunityMessageDetailsDto>> GetForCommunityAsync(long communityId);
    public Task<CommunityMessageDetailsDto> GetByIdAsync(long? id);
    public Task<CommunityMessageDetailsDto> UpdateMessage(EditCommunityMessageDto model);
}