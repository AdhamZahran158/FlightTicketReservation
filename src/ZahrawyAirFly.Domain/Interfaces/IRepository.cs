using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ZahrawyAirFly.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, object>>[]? includes = null);
        Task<T?> GetOneAsync(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, object>>[]? includes = null);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> Query();
        Task<T> AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<bool> ExistsAsync(string id);
        Task CommitAsync();

    }
}
