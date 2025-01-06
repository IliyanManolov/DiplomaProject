using HomeOwners.Domain.Models;

namespace HomeOwners.Application.Abstractions.Repositories;

public interface IPropertyRepository : IBaseEntityRepository<Property>
{
    public Task<IEnumerable<Property>> GetAllPropertiesByUserIdAsync(string userId);
    public Task<IEnumerable<Property>> GetAllCommunityPropertiesByUserIdAsync(string userId, string communityId);
    public Task<IEnumerable<Property>> GetAllPropertiesByEmailAsync(string userId);
    public Task<IEnumerable<Property>> GetAllCommunityPropertiesByEmailAsync(string userId, string communityId);
}
