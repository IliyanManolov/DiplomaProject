using HomeOwners.Domain.Models;

namespace HomeOwners.Application.Abstractions.Repositories;

public interface ICommunityRepository : IBaseEntityRepository<Community>
{
    public Task<Community?> GetByNameAsync(string name);
    public Task<bool> IsExistingNameAsync(string name);
} 
