using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Domain.Models;
using HomeOwners.Infrastructure.Database;

namespace HomeOwners.Infrastructure.Repositories;

internal class DuesCalculationRepository : BaseEntityRepository<DuesCalculation>, IDuesCalculationRepository
{
    public DuesCalculationRepository(DatabaseContext dbContext) : base(dbContext)
    {

    }
}