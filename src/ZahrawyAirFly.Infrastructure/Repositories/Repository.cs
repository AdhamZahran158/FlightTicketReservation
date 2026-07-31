using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ZahrawyAirFly.Domain.Interfaces;
using ZahrawyAirFly.Infrastructure.Data;

namespace ZahrawyAirFly.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, object>>[]? includes = null)
        {
            var dataSet = Query();
            if(includes is not null)
            {
                foreach (var item in includes)
                {
                    dataSet = dataSet.Include(item);
                }
            }
            if (predicate is not null)
            {
                dataSet = dataSet.Where(predicate);
            }
            return await dataSet.ToListAsync();
        }

        public async Task<T?> GetOneAsync(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, object>>[]? includes = null)
        => (await GetAsync(predicate,includes)).FirstOrDefault();

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)=> await _dbSet.CountAsync(predicate);
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public IQueryable<T> Query() => _dbSet.AsQueryable();

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }
        public void Update(T entity) => _dbSet.Update(entity);

        public void Delete(T entity) => _dbSet.Remove(entity);

        public async Task<bool> ExistsAsync(string id) => await _dbSet.AnyAsync(e => EF.Property<string>(e, "Id") == id);

        public async Task CommitAsync() => await _context.SaveChangesAsync();
    }
}
