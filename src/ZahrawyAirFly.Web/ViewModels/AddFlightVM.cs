using ZahrawyAirFly.Domain.Entities;

namespace ZahrawyAirFly.Web.ViewModels
{
    public class AddFlightVM
    {
        public List<Aircraft>? Aircrafts { get; set; }
        public List<Airport>? Airports { get; set; }
    }
}
