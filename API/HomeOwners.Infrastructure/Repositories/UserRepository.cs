using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Domain.Models;
using HomeOwners.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace HomeOwners.Infrastructure.Repositories;

internal class UserRepository : BaseEntityRepository<User>, IUserRepository
{
    public UserRepository(DatabaseContext dbContext)
        : base(dbContext)
    {
    }

    public override async Task<User?> GetByIdAsync(long? id)
    {
        return await Query
            .Include(x => x.ReferalCode)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await Query
            .Where(x => x.Username!.CompareTo(username) == 0)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await Query
            .Where(x => x.Email!.CompareTo(email) == 0)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsExistingEmailAsync(string email)
    {
        return await Query
            .AnyAsync(x => x.Email!.CompareTo(email) == 0);
    }

    public async Task<bool> IsExistingUsernameAsync(string username)
    {
        return await Query
            .AnyAsync(x => x.Username!.CompareTo(username) == 0);
    }
}
