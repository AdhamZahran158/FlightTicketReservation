using FlightTicketReservation.Utilities;

namespace FlightTicketReservation.Models
{
    public class Flight
    {
        public int Id { get; set; }
        public DateTime DepratureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public double DistanceKM { get; set; }
        public FlightStatus Status { get; set; }
        public int DepAirportId { get; set; }
        public int ArrAirportId { get; set; }
        public int AircraftId { get; set; }
        public Aircraft Aircraft { get; set; }
        public Airport ArrAirport { get; set; }
        public Airport DepAirport { get; set; }
    }
}
