using System;
using System.Collections.Generic;
using ZahrawyAirFly.Domain.Enums;
using ZahrawyAirFly.Shared.Base;

namespace ZahrawyAirFly.Domain.Entities
{
    public class Flight : BaseEntity
    {

        public string AircraftId { get; set; } = string.Empty;
        public virtual Aircraft Aircraft { get; set; } = null!;

        public string OriginAirportId { get; set; } = string.Empty;
        public virtual Airport OriginAirport { get; set; } = null!;

        public string DestinationAirportId { get; set; } = string.Empty;
        public virtual Airport DestinationAirport { get; set; } = null!;

        public string FlightNumber { get; set; } = string.Empty;
        public DateTime DepartureUtc { get; set; }
        public DateTime ArrivalUtc { get; set; }
        public string Gate { get; set; } = string.Empty;
        public string Terminal { get; set; } = string.Empty;
        public FlightStatus Status { get; set; }
        public decimal BasePriceEconomy { get; set; }
        public decimal BasePriceBusiness { get; set; }
        public decimal BasePriceFirst { get; set; }
        public int MaxBaggageKg { get; set; }
        public int FreeBaggageKg { get; set; }
        public decimal ExtraBaggagePerKg { get; set; }

        public virtual ICollection<FlightSeat> FlightSeats { get; set; } = new List<FlightSeat>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<FlightOffer> FlightOffers { get; set; } = new List<FlightOffer>();
    }
}