using ZahrawyAirFly.Domain.Entities;
using ZahrawyAirFly.Domain.Enums;

namespace ZahrawyAirFly.Web.ViewModels
{
    public class AdminHomeVM
    {
        public IEnumerable<Flight> Flights { get; set; } = new List<Flight>();
        public IEnumerable<Booking> Bookings { get; set; } = new List<Booking>();
        public List<Payment> Payments { get; set; } = new List<Payment>();
        public List<Tenant> Tenants { get; set; } = new List<Tenant>();

        public int TotalBookings => Bookings?.Count() ?? 0;
        public decimal TotalRevenue => Payments?.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount) ?? 0; public int ActiveFlights => Flights?.Count(f => f.DepartureUtc > DateTime.UtcNow && f.Status != FlightStatus.Cancelled) ?? 0;
        public Dictionary<BookingStatus, int> BookingStatusCounts => Bookings?
            .GroupBy(b => b.Status)
            .ToDictionary(g => g.Key, g => g.Count()) ?? new Dictionary<BookingStatus, int>();

        public List<(string Route, int Count)> TopRoutes => Bookings?
            .Where(b => b.Flight != null)
            .GroupBy(b => new { Origin = b.Flight.OriginAirport?.IataCode, Dest = b.Flight.DestinationAirport?.IataCode })
            .Select(g => (Route: $"{g.Key.Origin} → {g.Key.Dest}", Count: g.Count()))
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToList() ?? new List<(string, int)>();

        public Dictionary<string, decimal> MonthlyRevenue { get; set; } = new();
        public List<BookingLog> RecentLogs { get; set; } = new();
    }
}
