namespace FlightTicketReservation.Models
{
    public class Aircraft
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public int TotalSeats { get; set; }
        public List<Seat> Seats { get; set; }
        public List<Flight> Flights { get; set; }
    }
}
