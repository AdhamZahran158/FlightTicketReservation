using System;
using System.Collections.Generic;
using ZahrawyAirFly.Shared.Base;

namespace ZahrawyAirFly.Domain.Entities
{
    public class Offer : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxUsageCount { get; set; }
        public int CurrentUsageCount { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<FlightOffer> FlightOffers { get; set; } = new List<FlightOffer>();
    }
}
