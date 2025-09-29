using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Core.Interfaces.Repositories {
    public interface IRepository<TEntity> where TEntity : class {
        Task<TEntity?> FindAsync(ISpecification<TEntity> specification);
        Task<TEntity?> FindByIdAsync(Guid id);
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        void Remove(TEntity entity);
        Task RemoveByIdAsync(Guid id);
        void RemoveRange(IEnumerable<TEntity> entities);

    }
}
