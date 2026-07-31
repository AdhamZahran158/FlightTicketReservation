using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ZahrawyAirFly.Domain.Entities;

namespace ZahrawyAirFly.Domain.Interfaces
{
    public interface IFlightRepository : IRepository<Flight>
    {
        Task<Flight?> GetFlightWithDetailsAsync(Expression<Func<Flight, bool>> predicate);
        Task<List<Flight>?> GetFlightsWithDetailsAsync();
    }
}
