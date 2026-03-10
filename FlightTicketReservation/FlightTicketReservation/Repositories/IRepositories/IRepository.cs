using System.Linq.Expressions;

namespace FlightTicketReservation.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> CreateAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task CommitAsync();
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? include = null, bool tracked = true);
        Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? include = null, bool tracked = true);
    }
}
