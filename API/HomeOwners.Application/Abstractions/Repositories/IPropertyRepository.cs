using HomeOwners.Domain.Models;

namespace HomeOwners.Application.Abstractions.Repositories;

public interface IPropertyRepository : IBaseEntityRepository<Property>
{
    public Task<IEnumerable<Property>> GetAllPropertiesByUserIdAsync(long? userId);
    public Task<IEnumerable<Property>> GetAllCommunityPropertiesByUserIdAsync(long? userId, long? communityId);
    public Task<IEnumerable<Property>> GetAllCommunityPropertiesByCommunityIdAsync(long? communityId);
    public Task<IEnumerable<Property>> GetAllPropertiesByEmailAsync(string email);
    public Task<IEnumerable<Property>> GetAllCommunityPropertiesByEmailAsync(string email, long? communityId);
}
