namespace FlightTicketReservation.Models
{
    public class Airport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public List<Flight> DepartingFlights { get; set; }
        public List<Flight> ArrivingFlights { get; set; }
    }
}
