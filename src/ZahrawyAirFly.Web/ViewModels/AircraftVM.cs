namespace ZahrawyAirFly.Web.ViewModels
{
    public class AircraftVM
    {
        public string? Id { get; set; }
        public string Model { get; set; } = string.Empty;

        public string RegistrationCode { get; set; } = string.Empty;

        public int Rows { get; set; }

        public int SeatsPerRow { get; set; }

        public int TotalSeats => Rows * SeatsPerRow;

        public string? SeatLayoutJson { get; set; }

        public bool IsActive { get; set; } = true;

        public IFormFile? Img { get; set; }

        public int? MaxRangeKm { get; set; }

        public string? Manufacturer { get; set; }

        public DateTime? ManufactureDate { get; set; }
    }
}
