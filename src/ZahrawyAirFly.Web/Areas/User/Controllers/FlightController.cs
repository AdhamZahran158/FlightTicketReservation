using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using ZahrawyAirFly.Domain.Entities;
using ZahrawyAirFly.Domain.Interfaces;
using ZahrawyAirFly.Infrastructure.Utilities;

namespace ZahrawyAirFly.Web.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = SD.USER_ROLE)]
    public class FlightController : Controller
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IRepository<Airport> _airportRepository;

        public FlightController(
            IFlightRepository flightRepository,
            IRepository<Airport> airportRepository)
        {
            _flightRepository = flightRepository;
            _airportRepository = airportRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string originId, string destinationId, DateTime? departureDate)
        {
            ViewBag.Airports = await _airportRepository.GetAsync(a => a.IsActive);

            var query = await _flightRepository.GetAsync(
                 f => f.DepartureUtc >= DateTime.UtcNow, // only future flights
                includes: [ f => f.Aircraft, f => f.OriginAirport, f => f.DestinationAirport ]
            );

            if (!string.IsNullOrEmpty(originId))
                query = query.Where(f => f.OriginAirportId == originId);
            if (!string.IsNullOrEmpty(destinationId))
                query = query.Where(f => f.DestinationAirportId == destinationId);
            if (departureDate.HasValue)
            {
                var start = departureDate.Value.Date;
                var end = start.AddDays(1);
                query = query.Where(f => f.DepartureUtc >= start && f.DepartureUtc < end);
            }

            var flights = query.OrderBy(f => f.DepartureUtc).ToList();

            ViewBag.SelectedOriginId = originId;
            ViewBag.SelectedDestId = destinationId;
            ViewBag.SelectedDate = departureDate?.ToString("yyyy-MM-dd");

            return View(flights);
        }

        // NOT USED
        [HttpPost]
        public async Task<IActionResult> SearchResults(
            string fromAirportId,
            string toAirportId,
            DateTime departureDate)
        {
            var flights = await _flightRepository.GetAsync(
                f =>
                    f.OriginAirportId == fromAirportId &&
                    f.DestinationAirportId == toAirportId &&
                    f.DepartureUtc.Date == departureDate.Date &&
                    f.IsDeleted == false,
                includes: new Expression<Func<Flight, object>>[]
                {
                    x => x.OriginAirport,
                    x => x.DestinationAirport,
                    x => x.Aircraft
                });

            return View(flights);
        }

        // NOT USED
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var flight = await _flightRepository.GetOneAsync(
                f => f.Id == id,
                includes: new Expression<Func<Flight, object>>[]
                {
                    x => x.OriginAirport,
                    x => x.DestinationAirport,
                    x => x.Aircraft,
                    x => x.FlightSeats
                });

            if (flight == null)
                return NotFound();

            return View(flight);
        }


        [HttpGet]
        public async Task<IActionResult> Book(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var flight = await _flightRepository.GetFlightWithDetailsAsync(f => f.Id == id);

            if (flight == null)
                return NotFound();

            var flightSeats = flight.FlightSeats
          .OrderBy(fs => fs.Seat.Row)
          .ThenBy(fs => fs.Seat.Column)
          .ToList();

            ViewBag.Flight = flight;
            return View(flightSeats);
        }
    }
}
