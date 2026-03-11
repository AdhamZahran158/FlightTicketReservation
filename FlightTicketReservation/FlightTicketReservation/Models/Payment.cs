using FlightTicketReservation.Utilities;

namespace FlightTicketReservation.Models
{
    
    public class Payment
    {
        public int Id { get; set; }
        public double Ammount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime TransactionDate { get; set; }
        public int BookingId { get; set; }
        public Booking Booking { get; set; }
    }
}
