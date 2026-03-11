using FlightTicketReservation.Utilities;

namespace FlightTicketReservation.Models
{
    
    public class Seat
    {
        public int Id { get; set; }
        public int SeatNumber { get; set; }
        public int AircraftId { get; set; }
        public SeatClass Classtype { get; set; }
        public Aircraft Aircraft { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}
