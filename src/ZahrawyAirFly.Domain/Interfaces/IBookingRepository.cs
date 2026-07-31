using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZahrawyAirFly.Domain.Entities;

namespace ZahrawyAirFly.Domain.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<Booking?> GetBookingWithDetailsAsync(string bookingId);
        Task<IEnumerable<Booking>> GetUserBookingsWithDetailsAsync(string userId);
        Task<List<Booking>?> GetBookingsWithDetailsAsync();
    }
}
