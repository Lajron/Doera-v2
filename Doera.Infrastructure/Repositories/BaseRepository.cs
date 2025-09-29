using Doera.Core.Interfaces;
using Doera.Core.Interfaces.Repositories;
using Doera.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Infrastructure.Repositories {
    internal class BaseRepository<TEntity>(
            ApplicationDbContext _db
        ) : IRepository<TEntity> where TEntity : class {
        protected readonly DbSet<TEntity> _dbSet = _db.Set<TEntity>();

        public async Task<TEntity?> FindAsync(ISpecification<TEntity> specification) {
            var query = _dbSet.AsQueryable();

            query = specification.Includes
                .Aggregate(query, (current, include) => current.Include(include));

            query = specification.IncludeStrings
                .Aggregate(query, (current, include) => current.Include(include));

            if (specification.Expression is not null)
                query = query.Where(specification.Expression);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<TEntity?> FindByIdAsync(Guid id) {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync() {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(TEntity entity) {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities) {
            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(TEntity entity) {
            _dbSet.Update(entity);
        }

        public void Remove(TEntity entity) {
            _dbSet.Remove(entity);
        }

        public async Task RemoveByIdAsync(Guid id) {
            TEntity? entity = await _dbSet.FindAsync(id);
            if (entity != null)
                _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities) {
            _dbSet.RemoveRange(entities);
        }
    }
}
