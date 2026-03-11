using FlightTicketReservation.Utilities;

namespace FlightTicketReservation.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public double TotalPrice { get; set; }
        public int TripId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public Trip Trip { get; set; }
    }
}
