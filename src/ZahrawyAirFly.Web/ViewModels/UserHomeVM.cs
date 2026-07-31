using ZahrawyAirFly.Domain.Entities;

namespace ZahrawyAirFly.Web.ViewModels
{
    public class UserHomeVM
    {
        public List<Flight>? Flights { get; set; }
        public List<Booking>? Bookings { get; set; }
        public List<Airport>? Airports { get; set; }
    }
}
