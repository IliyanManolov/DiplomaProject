using HomeOwners.Application.DTOs.Property;

namespace HomeOwners.Application.Abstractions.Services;

public interface IPropertyService
{
    public Task<long> CreatePropertyAsync(CreatePropertyDto model);
    public Task<IEnumerable<PropertyShortDto>> GetAllForCommunityAsync(long communityId);
    public Task<IEnumerable<PropertyShortDto>> GetAllForUserInCommunityAsync(long communityId, long userId);
}