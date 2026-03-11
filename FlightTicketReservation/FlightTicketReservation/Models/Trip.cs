using FlightTicketReservation.Utilities;

namespace FlightTicketReservation.Models
{
    public class Trip
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public string UserId { get; set; }
        public TripType Type { get; set; }
        public ApplicationUser User { get; set; }
        public List<Flight> Flights { get; set; }
    }
}
