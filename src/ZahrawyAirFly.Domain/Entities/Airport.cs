using ZahrawyAirFly.Shared.Base;

namespace ZahrawyAirFly.Domain.Entities
{
    public class Airport : BaseEntity
    {
        public string IataCode { get; set; } = string.Empty;
        public string IcaoCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string Timezone { get; set; } = "UTC";
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
