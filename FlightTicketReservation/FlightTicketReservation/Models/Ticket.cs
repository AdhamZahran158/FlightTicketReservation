namespace FlightTicketReservation.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public SeatClass SeatClass { get; set; }
        public double TicketPrice {  get; set; }
        public int BookingId { get; set; }
        public int FlightId { get; set; }
        public int SeatId { get; set; }
        public Booking Booking { get; set; }
        public Flight Flight { get; set; }
        public Seat Seat { get; set; }
    }
}
