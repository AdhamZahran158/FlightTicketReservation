using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using ZahrawyAirFly.Domain.Entities;
using ZahrawyAirFly.Domain.Interfaces;
using ZahrawyAirFly.Infrastructure.Utilities;
using ZahrawyAirFly.Web.Models;

namespace ZahrawyAirFly.Web.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles=SD.ADMIN_ROLE)]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBookingRepository _bookingRepository;
    private readonly IFlightRepository _flightRepository;
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IRepository<BookingLog> _bookingLogRepository;
    private readonly UserManager<Tenant> _userManager;

    public HomeController(ILogger<HomeController> logger, IBookingRepository bookingRepository, IFlightRepository flightRepository, IRepository<Payment> paymentRepository, UserManager<Tenant> userManager, IRepository<BookingLog> bookingLogRepository)
    {
        _logger = logger;
        _bookingRepository = bookingRepository;
        _flightRepository = flightRepository;
        _paymentRepository = paymentRepository;
        _userManager = userManager;
        _bookingLogRepository = bookingLogRepository;
    }

    public async Task<IActionResult> Index()
    {
        var flights = await _flightRepository.GetFlightsWithDetailsAsync();
        var bookings = await _bookingRepository.GetBookingsWithDetailsAsync();
        var payments = await _paymentRepository.GetAsync(
            includes: [ p => p.Booking, p => p.Booking.Flight ]
        );
        var users = _userManager.Users.ToList();
        var bookLogs = await _bookingLogRepository.GetAllAsync();

        var adminHome = new AdminHomeVM
        {
            Flights = flights ?? Enumerable.Empty<Flight>(),
            Bookings = bookings ?? Enumerable.Empty<Booking>(),
            Payments = payments?.ToList() ?? new List<Payment>(),
            Tenants = users ?? new List<Tenant>(),
            RecentLogs = bookLogs.ToList()
        };

        return View(adminHome);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
