using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Domain.Models;
using HomeOwners.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace HomeOwners.Infrastructure.Repositories;

internal class DuesCalculationRepository : BaseEntityRepository<DuesCalculation>, IDuesCalculationRepository
{
    public DuesCalculationRepository(DatabaseContext dbContext) : base(dbContext)
    {

    }

    public override async Task<DuesCalculation> CreateAsync(DuesCalculation entity)
    {

        var createTime = DateTime.UtcNow;

        var existsForMonth = await Query.AnyAsync(x => x.CreateDate.Value.Year == createTime.Year && x.CreateDate.Value.Month == createTime.Month);

        if (existsForMonth)
            return null;

        return await base.CreateAsync(entity);
    }
}