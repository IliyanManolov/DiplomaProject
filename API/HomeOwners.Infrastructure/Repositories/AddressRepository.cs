using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Domain.Models;
using HomeOwners.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace HomeOwners.Infrastructure.Repositories;

internal class AddressRepository : BaseEntityRepository<Address>, IAddressRepository
{
    public AddressRepository(DatabaseContext dbContext) : base(dbContext)
    {

    }

    public async Task<bool> ValidateAddress(string country, string city, string street, long? building, long? apartment)
    {
        return await Query
            .AnyAsync(x => (x.Country == country)
                    && (x.City == city)
                    && (x.StreetAddress == street)
                    && (x.BuildingNumber == building)
                    && (x.ApartmentNumber == apartment));
    }
}
