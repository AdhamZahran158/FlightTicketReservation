using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZahrawyAirFly.Domain.Entities;
using ZahrawyAirFly.Domain.Interfaces;
using ZahrawyAirFly.Infrastructure.Data;

namespace ZahrawyAirFly.Infrastructure.Repositories
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        public BookingRepository(AppDbContext context) : base(context) { }

        public async Task<Booking?> GetBookingWithDetailsAsync(string bookingId)
        {
            return await Query()
                .Include(b => b.Flight)
                    .ThenInclude(f => f.OriginAirport)
                .Include(b => b.Flight)
                    .ThenInclude(f => f.DestinationAirport)
                .Include(b => b.Flight)
                    .ThenInclude(f => f.Aircraft)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.FlightSeat)
                        .ThenInclude(fs => fs.Seat)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
        }

        public async Task<IEnumerable<Booking>> GetUserBookingsWithDetailsAsync(string userId)
        {
            return await Query()
                .Where(b => b.TenantId == userId)
                .Include(b => b.Flight)
                    .ThenInclude(f => f.OriginAirport)
                .Include(b => b.Flight)
                    .ThenInclude(f => f.DestinationAirport)
                .Include(b => b.Flight)
                    .ThenInclude(f => f.Aircraft)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.FlightSeat)
                        .ThenInclude(fs => fs.Seat)
                .ToListAsync();
        }

        public async Task<List<Booking>?> GetBookingsWithDetailsAsync()
        {
            return await Query()
                .Include(b => b.Flight)
                    .ThenInclude(f => f.OriginAirport)
                .Include(b => b.Flight)
                    .ThenInclude(f => f.DestinationAirport)
                .Include(b => b.Flight)
                    .ThenInclude(f => f.Aircraft)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.FlightSeat)
                        .ThenInclude(fs => fs.Seat)
                .ToListAsync();
        }
    }
}
