using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ZahrawyAirFly.Domain.Entities;
using ZahrawyAirFly.Domain.Interfaces;
using ZahrawyAirFly.Infrastructure.Data;

namespace ZahrawyAirFly.Infrastructure.Repositories
{
    public class FlightRepository : Repository<Flight>, IFlightRepository
    {
        public FlightRepository(AppDbContext context) : base(context) { }

        public async Task<Flight?> GetFlightWithDetailsAsync(Expression<Func<Flight, bool>> predicate)
        {
            return await Query()
                .Include(f => f.OriginAirport)
                .Include(f => f.DestinationAirport)
                .Include(f => f.Aircraft)
                .Include(f => f.FlightSeats)
                    .ThenInclude(fs => fs.Seat)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<List<Flight>?> GetFlightsWithDetailsAsync()
        {
            return await(Query()
                .Include(f => f.OriginAirport)
                .Include(f => f.DestinationAirport)
                .Include(f => f.Aircraft)
                .Include(f => f.FlightSeats)
                    .ThenInclude(fs => fs.Seat)).Skip(0).Take(30)
                .ToListAsync();
        }
    }
}
