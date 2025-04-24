using HomeOwners.Application.DTOs.Community;

namespace HomeOwners.Application.Abstractions.Services;

public interface ICommunityService
{
    public Task<long> CreateCommunityAsync(CreateCommunityDto model);
    public Task<CommunityDetailsDto> GetCommunityDetailsAsync(long? id);
    public Task<IEnumerable<CommunityDetailsDto>> GetAllCommunities();
    public Task<IEnumerable<CommunityDetailsDto>> GetAllForUser(long userId);
}