using System;
using ZahrawyAirFly.Shared.Base;

namespace ZahrawyAirFly.Domain.Entities
{
    public class FlightOffer : BaseEntity
    {
        public string FlightId { get; set; }
        public virtual Flight Flight { get; set; } = null!;
        public string OfferId { get; set; }
        public virtual Offer Offer { get; set; } = null!;
    }
}
