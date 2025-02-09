using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Domain.Models;
using HomeOwners.Infrastructure.Database;

namespace HomeOwners.Infrastructure.Repositories;

internal class AddressRepository : BaseEntityRepository<Address>, IAddressRepository
{
    public AddressRepository(DatabaseContext dbContext) : base(dbContext)
    {
    }
}
