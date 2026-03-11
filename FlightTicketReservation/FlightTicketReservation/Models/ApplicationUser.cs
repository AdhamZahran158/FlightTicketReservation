using Microsoft.AspNetCore.Identity;

namespace FlightTicketReservation.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PassportNum { get; set; }
        public string? Nationality { get; set; }
    }
}
