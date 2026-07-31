using System.Collections.Generic;
using ZahrawyAirFly.Shared.Base;

namespace ZahrawyAirFly.Domain.Entities
{
    public class Aircraft : BaseEntity
    {
        public string Model { get; set; } = string.Empty;

        public string RegistrationCode { get; set; } = string.Empty;

        public int Rows { get; set; }

        public int SeatsPerRow { get; set; }

        public int TotalSeats => Rows * SeatsPerRow;

        public string? SeatLayoutJson { get; set; }

        public bool IsActive { get; set; } = true;

        public string Img { get; set; } = string.Empty;

        public int? MaxRangeKm { get; set; }

        public string? Manufacturer { get; set; }

        public DateTime? ManufactureDate { get; set; }

        public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}