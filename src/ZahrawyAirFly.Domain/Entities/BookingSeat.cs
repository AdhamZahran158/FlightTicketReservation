using System;
using ZahrawyAirFly.Shared.Base;

namespace ZahrawyAirFly.Domain.Entities
{
    public class BookingSeat : BaseEntity
    {
        public string BookingId { get; set; }
        public virtual Booking Booking { get; set; } = null!;
        public string FlightSeatId { get; set; }
        public virtual FlightSeat FlightSeat { get; set; } = null!;
        public string PassengerName { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
    }
}
