using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using ZahrawyAirFly.Domain.Entities;
using ZahrawyAirFly.Domain.Interfaces;
using ZahrawyAirFly.Infrastructure.Utilities;

namespace ZahrawyAirFly.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ADMIN_ROLE)]
    public class BookingController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly UserManager<Tenant> userManager;
        private readonly IEmailSender emailSender;
        private readonly IMemoryCache _cache;

        public BookingController(IBookingRepository bookingRepository, UserManager<Tenant> userManager, IEmailSender emailSender, IMemoryCache memoryCache)
        {
            _bookingRepository = bookingRepository;
            this.userManager = userManager;
            this.emailSender = emailSender;
            this._cache = memoryCache;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bookings = await _bookingRepository.GetBookingsWithDetailsAsync();
            var userEmails = new Dictionary<string, string>();
            foreach (var b in bookings)
            {
                var user = await userManager.FindByIdAsync(b.TenantId);
                userEmails[b.Id] = user?.Email ?? "N/A";
            }
            ViewBag.UserEmails = userEmails;
            return View(bookings);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserBookingsByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return View();
            }
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                TempData["error"] = "User not found, Please check the email and try again";
                return View();
            }
            var userBookings = await _bookingRepository.GetUserBookingsWithDetailsAsync(user.Id);
            ViewBag.UserEmail = user.Email;
            return View(userBookings);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationCode(string bookingId)
        {
            var booking = await _bookingRepository.GetBookingWithDetailsAsync(bookingId);
            if (booking == null) return NotFound();

            var user = await userManager.FindByIdAsync(booking.TenantId);
            if (user == null) return NotFound();

            var code = new Random().Next(100000, 999999).ToString();
            _cache.Set($"BookingVerify_{bookingId}", code, TimeSpan.FromMinutes(10));

            // Send email
            var subject = "Booking Modification Verification Code";
            var htmlMessage = $@"
        <h3>Booking Modification Request</h3>
        <p>An admin has requested to modify your booking <strong>{booking.BookingRef}</strong>.</p>
        <p>If you approve, please provide the following code to the admin:</p>
        <h2 style='background:#f8b600; padding:10px; display:inline-block; border-radius:5px;'>{code}</h2>
        <p>This code expires in 10 minutes.</p>
        <p>If you did not request this, please ignore this email.</p>
    ";
            await emailSender.SendEmailAsync(user.Email, subject, htmlMessage);

            TempData["success"] = "Verification code sent to the user's email.";
            return RedirectToAction(nameof(ModifyBooking), new { id = bookingId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(string bookingId, string code)
        {
            var cachedCode = _cache.Get<string>($"BookingVerify_{bookingId}");
            if (string.IsNullOrEmpty(cachedCode) || cachedCode != code)
            {
                TempData["error"] = "Invalid or expired code. Please request a new one.";
                return RedirectToAction(nameof(ModifyBooking), new { id = bookingId });
            }

            // Remove the code so it can't be reused
            _cache.Remove($"BookingVerify_{bookingId}");

            // Mark as verified in ViewBag
            ViewBag.IsVerified = true;
            // Reload booking to pass to view
            var booking = await _bookingRepository.GetBookingWithDetailsAsync(bookingId);
            ViewBag.UserEmail = (await userManager.FindByIdAsync(booking.TenantId))?.Email;
            return View(nameof(ModifyBooking), booking);
        }

        [HttpGet]
        public async Task<IActionResult> ModifyBooking(string id)
        {
            var booking = await _bookingRepository.GetBookingWithDetailsAsync(id);
            if (booking is null)
            {
                TempData["error"] = "Did not find the booking";
                return RedirectToAction("Index");
            }
            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> ModifyBooking(Booking booking)
        {
            _bookingRepository.Update(booking);
            await _bookingRepository.CommitAsync();
            TempData["success"] = "Updated Booking Successfully";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CancelBooking(string id)
        {
            var booking = await _bookingRepository.GetBookingWithDetailsAsync(id);
            if (booking is null)
            {
                TempData["error"] = "Failed to cancel the booking";
                RedirectToAction("Index");
            }
            foreach (var item in booking.BookingSeats)
            {
                item.FlightSeat.Status = Domain.Enums.SeatStatus.Available;
            }
            booking.Status = Domain.Enums.BookingStatus.Cancelled;
            TempData["success"] = "Booking Cancelled Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
