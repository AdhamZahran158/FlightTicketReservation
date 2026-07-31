using System.ComponentModel.DataAnnotations;

namespace ZahrawyAirFly.Web.ViewModels
{
    public class VerifyEmailVM
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
