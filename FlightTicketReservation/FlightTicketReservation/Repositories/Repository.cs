using FlightTicketReservation.DbAccess;
using FlightTicketReservation.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FlightTicketReservation.Repositories
{
    public class Repository<T>:IRepository<T> where T : class
    {
        private ApplicationDbContext _context;
        private DbSet<T> _dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> CreateAsync(T entity)
        {
            var created = await _dbSet.AddAsync(entity);
            return entity;
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            var deleted =  _dbSet.Remove(entity);
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? include = null, bool tracked = true )
        {
            var entities = _dbSet.AsQueryable();

            if(expression != null)
            {
                entities = entities.Where(expression);
            }
            if(include != null)
            {
                foreach (var item in include)
                {
                    entities = entities.Include(item);
                }
            }
            if(!tracked)
                entities = entities.AsNoTracking();
            return await entities.ToListAsync();
        }

        public async Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? include = null, bool tracked = true)
        {
            return (await GetAsync(expression, include, tracked)).FirstOrDefault();
        }
    }
}
