using System.ComponentModel.DataAnnotations;

namespace ZahrawyAirFly.Web.ViewModels
{
    public class SignUpVM
    {
        [Required]
        [Display(Name = "Company Name")]
        [StringLength(100, MinimumLength = 2)]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Name")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Subdomain")]
        [StringLength(50, MinimumLength = 3)]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Only lowercase letters, numbers, and hyphens allowed")]
        public string Subdomain { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Preferred Currency")]
        public string Currency { get; set; } = "USD";
        public string Passport { get; set; }
    }
}
