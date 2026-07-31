using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ZahrawyAirFly.Web.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class ProfileController : Controller
    {
        private readonly UserManager<Tenant> userManager;
        private readonly SignInManager<Tenant> signInManager;
        public ProfileController(UserManager<Tenant> userManager, SignInManager<Tenant> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login","Account",new {area="Identity"});

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(Tenant profile)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account", new { area = "Identity" });

            user.Name = profile.Name;
            user.CompanyName = profile.CompanyName;
            user.PassportNumber = profile.PassportNumber;
            user.Currency = profile.Currency;
            user.LogoUrl = profile.LogoUrl;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                TempData["error"] = string.Join(", ", result.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Index));
            }
            TempData["success"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> UpdatePassword()
        {
            var user = await userManager.GetUserAsync (User);
            if (user is null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(string password, string oldPassword)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            if (string.IsNullOrEmpty(password))
            {
                TempData["error"] = "Password can not be empty";
                return View(user);
            }
            var result = await userManager.ChangePasswordAsync(user, password, oldPassword);
            if (!result.Succeeded)
            {
                TempData["error"] = string.Join(", ", result.Errors.Select(e => e.Description));
                return View();
            }
            TempData["success"] = "Updated Password Successfully";
            return RedirectToAction(nameof (Index));
        }

        public async Task<IActionResult> DeleteAccount(string password)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            var checkPassword = await userManager.CheckPasswordAsync(user, password);

            if (!checkPassword)
            {
                TempData["error"] = "Wrong password, Account is not deleted";
                return RedirectToAction(nameof(Index));
            }

            await signInManager.SignOutAsync();
            var res = await userManager.DeleteAsync(user);
            if (res.Succeeded)
                TempData["success"] = "Deleted Profile Successfully";
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }
    }
}
