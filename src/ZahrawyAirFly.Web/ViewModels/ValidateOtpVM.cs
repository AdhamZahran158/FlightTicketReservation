using System.ComponentModel.DataAnnotations;

namespace ZahrawyAirFly.Web.ViewModels
{
    public class ValidateOtpVM
    {
        public string UserId { get; set; }

        [Required]
        [Display(Name = "OTP Code")]
        [StringLength(6, MinimumLength = 6)]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "OTP must be 6 digits")]
        public string OTP { get; set; }
    }
}
