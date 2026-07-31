using System;
using ZahrawyAirFly.Domain.Enums;
using ZahrawyAirFly.Shared.Base;

namespace ZahrawyAirFly.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public string BookingId { get; set; }
        public virtual Booking Booking { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public PaymentMethod Method { get; set; }
        public string TransactionRef { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
