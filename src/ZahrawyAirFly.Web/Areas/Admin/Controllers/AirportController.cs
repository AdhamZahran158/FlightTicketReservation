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
    public class AirportController : Controller
    {
        private readonly IRepository<Airport> _airportRepository;

        public AirportController(IRepository<Airport> airportRepository)
        {
            _airportRepository = airportRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var airports = await _airportRepository.GetAllAsync();
            return View(airports);
        }

        [HttpGet]
        public IActionResult AddAirport()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddAirport(Airport airport)
        {
            try
            {
                await _airportRepository.AddAsync(airport);
                await _airportRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateAirport(string id)
        {
            var airport = await _airportRepository.GetOneAsync(a => a.Id == id);
            if (airport is null)
            {
                return NotFound();
            }
            return View(airport);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAirport(Airport airport)
        {
            _airportRepository.Update(airport);
            await _airportRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteAirport(string id)
        {
            var airport = await _airportRepository.GetOneAsync(a => a.Id == id);
            if (airport is null)
                return NotFound();
            _airportRepository.Delete(airport);
            await _airportRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
