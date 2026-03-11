namespace FlightTicketReservation.Models
{
    public class Baggage
    {
        public int Id { get; set; }
        public double Weight { get; set; }
        public double? ExtraFee { get; set; } 
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }
}
