using System;
using ZahrawyAirFly.Domain.Enums;
using ZahrawyAirFly.Shared.Base;

namespace ZahrawyAirFly.Domain.Entities
{
    public class FlightSeat : BaseEntity
    {
        public string FlightId { get; set; }
        public virtual Flight Flight { get; set; } = null!;
        public string SeatId { get; set; }
        public virtual Seat Seat { get; set; } = null!;
        public SeatStatus Status { get; set; }
        public decimal PriceOverride { get; set; }
        public string? LockedByUserId { get; set; }
        public DateTime? LockedUntil { get; set; }
    }
}
