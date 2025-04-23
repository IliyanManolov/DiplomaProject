using HomeOwners.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeOwners.Application.Abstractions.Repositories;

public interface IBaseEntityRepository<TEntity>
    where TEntity : IDomainEntity
{
    public Task<TEntity?> GetByIdAsync(long? id);
    public Task<IEnumerable<TEntity>> GetAllAsync();
    public Task<IEnumerable<TEntity>> CreateBulkAsync(IEnumerable<TEntity> entities);
    public Task<TEntity> CreateAsync(TEntity entity);
    public Task<TEntity> DeleteAsync(TEntity entity);
    public Task<TEntity> UpdateAsync(TEntity entity);
}
