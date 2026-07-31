using System;
using System.Collections.Generic;
using ZahrawyAirFly.Domain.Enums;
using ZahrawyAirFly.Shared.Base;

namespace ZahrawyAirFly.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public string BookingRef { get; set; } = string.Empty;
        public string EncryptedRef { get; set; } = string.Empty;

        public string TenantId { get; set; } = string.Empty;
        public virtual Tenant Tenant { get; set; } = null!;

        public string FlightId { get; set; } = string.Empty;
        public virtual Flight Flight { get; set; } = null!;

        public string? DiscountId { get; set; }
        public virtual Discount? Discount { get; set; }

        public BookingStatus Status { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal FeeAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int BaggageKg { get; set; }
        public bool AgreedToTerms { get; set; }
        public DateTime BookedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string CancellationReason { get; set; } = string.Empty;

        public virtual ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<BookingLog> BookingLogs { get; set; } = new List<BookingLog>();
    }
}