using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ZahrawyAirFly.Domain.Entities;
using ZahrawyAirFly.Domain.Interfaces;
using ZahrawyAirFly.Infrastructure.Utilities;

namespace ZahrawyAirFly.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ADMIN_ROLE)]
    public class FlightController : Controller
    {
        private readonly IRepository<Flight> _flightRepository;
        private readonly IRepository<Airport> _airportRepository;
        private readonly IRepository<Aircraft> _aircraftRepository;
        private readonly IRepository<FlightSeat> _flightSeatRepository;

        public FlightController(IRepository<Flight> flightRepository, IRepository<Airport> airportRepository, IRepository<Aircraft> aircraftRepository, IRepository<FlightSeat> flightSeatRepository)
        {
            _flightRepository = flightRepository;
            _airportRepository = airportRepository;
            _aircraftRepository = aircraftRepository;
            _flightSeatRepository = flightSeatRepository;
        }

        public async Task<IActionResult> Index()
        {
            var flights = await _flightRepository.GetAsync(includes: [f => f.Aircraft, f => f.DestinationAirport, f => f.OriginAirport]);
            return View(flights);
        }

        [HttpGet]
        public async Task<IActionResult> AddFlight()
        {
            var aircrafts = await _aircraftRepository.GetAllAsync();
            var airports = await _airportRepository.GetAllAsync();
            var addFlightVm = new AddFlightVM() { 
            Aircrafts = aircrafts.ToList(),
            Airports= airports.ToList()
            };

            return View(addFlightVm);
        }

        [HttpPost]
        public async Task<IActionResult> AddFlight(Flight flight)
        {
            var craft = await _aircraftRepository.GetOneAsync(a => a.Id == flight.AircraftId, includes: [a=>a.Seats]);
            await _flightRepository.AddAsync(flight);
            foreach (var item in craft.Seats)
            {
                await _flightSeatRepository.AddAsync(new()
                {
                    SeatId = item.Id,
                    FlightId = flight.Id,
                    Status = Domain.Enums.SeatStatus.Available
                });
            }
            await _flightRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateFlight(string id)
        {
            var aircrafts = await _aircraftRepository.GetAllAsync();
            var airports = await _airportRepository.GetAllAsync();
            ViewBag.Aircrafts = aircrafts.ToList();
            ViewBag.Airports = airports.ToList();
            var flight = await _flightRepository.GetOneAsync(f => f.Id == id);
            if( flight is null)
                return NotFound();
            return View(flight);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFlight(Flight flight)
        {
            var oldFlight = await _flightRepository.GetOneAsync(f => f.Id == flight.Id, [f=>f.FlightSeats]);
            if( oldFlight is null)
                return NotFound();

            if(oldFlight.AircraftId != flight.AircraftId)
            {
                var newAircraft = await _aircraftRepository.GetOneAsync(a=>a.Id == flight.AircraftId, [a=>a.Seats]);
                foreach (var item in oldFlight.FlightSeats)
                {
                    _flightSeatRepository.Delete(item);
                }
                foreach (var item in newAircraft.Seats)
                {
                    await _flightSeatRepository.AddAsync(new() { SeatId = item.Id,
                        FlightId = flight.Id,
                        Status = Domain.Enums.SeatStatus.Available
                    });

                }
            }
            _flightRepository.Update(flight);
            await _flightRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteFlight(string id)
        {
            var flight = await _flightRepository.GetOneAsync(f=>f.Id == id, [f=>f.FlightSeats]);
            if (flight is null)
            {
                return NotFound();
            }
            foreach (var item in flight.FlightSeats )
            {
                _flightSeatRepository.Delete(item);
            }
            _flightRepository.Delete(flight);
            await _flightRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
