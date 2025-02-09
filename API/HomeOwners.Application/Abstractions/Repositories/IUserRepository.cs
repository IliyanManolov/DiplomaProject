using HomeOwners.Domain.Models;

namespace HomeOwners.Application.Abstractions.Repositories;

public interface IUserRepository : IBaseEntityRepository<User>
{
    public Task<User?> GetByUsernameAsync(string username);
    public Task<User?> GetUserByEmail(string email);
    public Task<bool> IsExistingUsernameAsync(string username);
    public Task<bool> IsExistingEmailAsync(string email);
}
