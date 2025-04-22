using HomeOwners.Domain.Models;

namespace HomeOwners.Application.Abstractions.Repositories;

public interface IAddressRepository : IBaseEntityRepository<Address>
{
    public Task<bool> ValidateAddress(string country, string city, string street, long? building, long? apartment);
}