using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZahrawyAirFly.Domain.Entities;
using ZahrawyAirFly.Domain.Interfaces;
using ZahrawyAirFly.Web.Models;

namespace ZahrawyAirFly.Web.Areas.User.Controllers;
[Area("User")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IFlightRepository _flightRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IRepository<Airport> _airportRepository;
    private readonly UserManager<Tenant> _userManager;

    public HomeController(ILogger<HomeController> logger, IFlightRepository flightRepository, IBookingRepository bookingRepository, UserManager<Tenant> userManager, IRepository<Airport> airportRepository)
    {
        _logger = logger;
        _flightRepository = flightRepository;
        _bookingRepository = bookingRepository;
        _userManager = userManager;
        _airportRepository = airportRepository;
    }

    public async Task<IActionResult> Index()
    {
        var airports = await _airportRepository.GetAllAsync();
        var flights = (await _flightRepository.GetFlightsWithDetailsAsync()).Skip(0).Take(3);
        var user = await _userManager.GetUserAsync(User);
        var bookings = user is not null ? await _bookingRepository.GetUserBookingsWithDetailsAsync(user.Id) : null;
        var userHome = new UserHomeVM()
        {
            Bookings = bookings is null? null: bookings.ToList(),
            Flights = flights is null? null: flights.ToList(),
            Airports = airports is null? null: airports.ToList(),
        };
        return View(userHome);
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
