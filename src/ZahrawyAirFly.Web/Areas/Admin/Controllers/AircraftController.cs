using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ZahrawyAirFly.Domain.Entities;
using ZahrawyAirFly.Domain.Enums;
using ZahrawyAirFly.Domain.Interfaces;
using ZahrawyAirFly.Infrastructure.Utilities;

namespace ZahrawyAirFly.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ADMIN_ROLE)]
    public class AircraftController : Controller
    {
        private readonly IRepository<Aircraft> _aircraftRepository;
        private readonly IRepository<Seat> _seatRepository;

        public AircraftController(IRepository<Aircraft> aircraftRepository, IRepository<Seat> seatRepository)
        {
            _aircraftRepository = aircraftRepository;
            _seatRepository = seatRepository;
        }

        [HttpGet]
        public async Task<IActionResult> AircraftIndex()
        {
            var aircrafts = await _aircraftRepository.GetAllAsync();
            return View(aircrafts);
        }

        [HttpGet]
        public async Task<IActionResult> AircraftDetails(string id)
        {
            var aircraft = await _aircraftRepository.GetOneAsync(a => a.Id == id);
            return View(aircraft);
        }

        [HttpGet]
        public IActionResult AddAircraft()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddAircraftAsync(AircraftVM aircraft)
        {
            try
            {
                Aircraft newCraft;
                if (aircraft.Img is not null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(aircraft.Img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\Aircrafts", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        aircraft.Img.CopyTo(stream);
                    }
                    newCraft = await _aircraftRepository.AddAsync(new Aircraft()
                    {
                        Model = aircraft.Model,
                        RegistrationCode = aircraft.RegistrationCode,
                        Rows = aircraft.Rows,
                        SeatLayoutJson = aircraft.SeatLayoutJson,
                        SeatsPerRow = aircraft.SeatsPerRow,
                        Img = fileName,
                        IsActive = aircraft.IsActive,
                        ManufactureDate = aircraft.ManufactureDate,
                        Manufacturer = aircraft.Manufacturer,
                        MaxRangeKm = aircraft.MaxRangeKm,
                    });
                }
                else
                {
                    newCraft = await _aircraftRepository.AddAsync(new Aircraft()
                    {
                        Model = aircraft.Model,
                        RegistrationCode = aircraft.RegistrationCode,
                        Rows = aircraft.Rows,
                        SeatLayoutJson = aircraft.SeatLayoutJson,
                        SeatsPerRow = aircraft.SeatsPerRow,
                        Img = "default.jpg",
                        IsActive = aircraft.IsActive,
                        ManufactureDate = aircraft.ManufactureDate,
                        Manufacturer = aircraft.Manufacturer,
                        MaxRangeKm = aircraft.MaxRangeKm,
                    });
                }
                int seatNum = 1;
                for (int i = 0; i < aircraft.Rows; i++)
                {
                    for (int j = 0; j < aircraft.SeatsPerRow; j++)
                    {
                        await _seatRepository.AddAsync(new()
                        {
                            IsWindow = (j == 0 || j == aircraft.SeatsPerRow - 1),
                            IsAisle = (j == 2 || j == aircraft.SeatsPerRow - 3),
                            Row = j + 1,
                            Column = ('A' + j).ToString(),
                            Zone = i <= aircraft.Rows / 3 ? "First Class" : i <= 2 * aircraft.Rows / 3 ? "Bussiness" : "Economy",
                            Class = i <= aircraft.Rows / 3 ? SeatClass.First : i <= 2 * aircraft.Rows / 3 ? SeatClass.Business : SeatClass.Economy,
                            AircraftId = newCraft.Id,
                            SeatNumber = (seatNum).ToString()
                        });
                        seatNum++;
                    }

                }
                await _aircraftRepository.CommitAsync();
                return RedirectToAction(nameof(AircraftIndex));
            }
            catch(Exception ex)
            {
                TempData["error"] = ex.ToString();
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateAircraft(string id)
        {
            var aircraft = await _aircraftRepository.GetOneAsync(a => a.Id == id);
            if (aircraft is null)
            {
                return NotFound();
            }
            return View(aircraft);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAircraft(AircraftVM aircraftVM)
        {
            var oldCraft = await _aircraftRepository.GetOneAsync(a => a.Id == aircraftVM.Id);

            if (aircraftVM.Img is not null)
            {
                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(aircraftVM.Img.FileName);
                var newPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\Aircrafts", newFileName);
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\Aircrafts", oldCraft.Img);

                using (var newStream = System.IO.File.Create(newPath))
                {
                    aircraftVM.Img.CopyTo(newStream);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
                oldCraft.Img = newFileName;
                oldCraft.Manufacturer = aircraftVM.Manufacturer;
                oldCraft.ManufactureDate = aircraftVM.ManufactureDate;
                oldCraft.Model = aircraftVM.Model;
                oldCraft.MaxRangeKm = aircraftVM.MaxRangeKm;
                oldCraft.RegistrationCode = aircraftVM.RegistrationCode;
                oldCraft.Rows = aircraftVM.Rows;
                oldCraft.SeatLayoutJson = aircraftVM.SeatLayoutJson;
                oldCraft.SeatsPerRow = aircraftVM.SeatsPerRow;
                oldCraft.UpdatedAt = DateTime.UtcNow;
                oldCraft.IsActive = aircraftVM.IsActive;
                _aircraftRepository.Update(oldCraft);
            }
            else
            {
                oldCraft.Manufacturer = aircraftVM.Manufacturer;
                oldCraft.ManufactureDate = aircraftVM.ManufactureDate;
                oldCraft.Model = aircraftVM.Model;
                oldCraft.MaxRangeKm = aircraftVM.MaxRangeKm;
                oldCraft.RegistrationCode = aircraftVM.RegistrationCode;
                oldCraft.Rows = aircraftVM.Rows;
                oldCraft.SeatLayoutJson = aircraftVM.SeatLayoutJson;
                oldCraft.SeatsPerRow = aircraftVM.SeatsPerRow;
                oldCraft.UpdatedAt = DateTime.UtcNow;
                oldCraft.IsActive = aircraftVM.IsActive;
                _aircraftRepository.Update(oldCraft);
            }
            await _aircraftRepository.CommitAsync();
            return RedirectToAction(nameof(AircraftIndex));
        }

        public async Task<IActionResult> DeleteAircraft(string id)
        {
            var aircraft = await _aircraftRepository.GetOneAsync(a => a.Id == id);
            if (aircraft is null)
            {
                return NotFound();
            }
            if (aircraft.Img != "default.jpg")
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\Aircrafts", aircraft.Img);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            _aircraftRepository.Delete(aircraft);
            await _aircraftRepository.CommitAsync();
            return RedirectToAction(nameof(AircraftIndex));
        }
    }
}
