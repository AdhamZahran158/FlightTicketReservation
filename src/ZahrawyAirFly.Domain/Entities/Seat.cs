using System;
using ZahrawyAirFly.Domain.Enums;
using ZahrawyAirFly.Shared.Base;

namespace ZahrawyAirFly.Domain.Entities
{
    public class Seat : BaseEntity
    {
        public string AircraftId { get; set; }
        public virtual Aircraft Aircraft { get; set; } = null!;
        public string SeatNumber { get; set; } = string.Empty;
        public int Row { get; set; }
        public string Column { get; set; } = string.Empty;
        public SeatClass Class { get; set; }
        public string Zone { get; set; } = string.Empty;
        public bool IsExitRow { get; set; }
        public bool IsWindow { get; set; }
        public bool IsAisle { get; set; }
    }
}
