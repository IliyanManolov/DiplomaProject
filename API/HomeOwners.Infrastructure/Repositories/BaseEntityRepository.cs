using HomeOwners.Application.Abstractions.Repositories;
using HomeOwners.Domain.Abstractions;
using HomeOwners.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace HomeOwners.Infrastructure.Repositories;

internal abstract class BaseEntityRepository<TEntity> : IBaseEntityRepository<TEntity> where TEntity : class, IDomainEntity
{
    private readonly DatabaseContext _dbContext;
    protected IQueryable<TEntity> Query => _dbContext.Set<TEntity>();

    protected BaseEntityRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
        => await _dbContext.Set<TEntity>().ToListAsync();

    public virtual async Task<TEntity?> GetByIdAsync(long? id)
        => await _dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);

    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        entity.CreateDate = DateTime.UtcNow;
        await _dbContext.Set<TEntity>().AddAsync(entity);

        await Save();

        return entity;
    }
    public async Task<IEnumerable<TEntity>> CreateBulkAsync(IEnumerable<TEntity> entities)
    {
        await _dbContext.Set<TEntity>().AddRangeAsync(entities);
        
        await Save();

        return entities;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity.UpdateDate = DateTime.UtcNow;
        _dbContext.Set<TEntity>()
            .Entry(entity).State = EntityState.Modified;

        await Save();
        return entity;
    }

    public async Task<IEnumerable<TEntity>> UpdateBulkAsync(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.UpdateDate = DateTime.UtcNow;
            _dbContext.Set<TEntity>().Entry(entity).State = EntityState.Modified;
        }

        await Save();
        return entities;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        _dbContext.Set<TEntity>().Remove(entity);
        await Save();

        return entity;
    }

    // Abstracted to make getting metrics in the future easier
    protected async Task<int> Save()
    {
        var saves = await _dbContext.SaveChangesAsync();

        return saves;
    }
}
