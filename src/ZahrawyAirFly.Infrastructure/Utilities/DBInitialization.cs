using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ZahrawyAirFly.Domain.Entities;
using ZahrawyAirFly.Domain.Enums;
using ZahrawyAirFly.Domain.Interfaces;
using ZahrawyAirFly.Infrastructure;
using ZahrawyAirFly.Infrastructure.Data;

namespace ZahrawyAirFly.Infrastructure.Utilities
{
    public class DBIntialization : IDBInitialization
    {
        private readonly UserManager<Tenant> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public DBIntialization(
            UserManager<Tenant> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public void Initialize()
        {
            // =========================
            // 1. Create Roles
            // =========================

            if (!_roleManager.Roles.Any())
            {
                _roleManager
                    .CreateAsync(new IdentityRole(SD.ADMIN_ROLE))
                    .GetAwaiter()
                    .GetResult();

                _roleManager
                    .CreateAsync(new IdentityRole(SD.USER_ROLE))
                    .GetAwaiter()
                    .GetResult();
            }


            // =========================
            // 2. Create Admin User
            // =========================

            var adminUser = _userManager
                .FindByNameAsync("Admin@Flight")
                .GetAwaiter()
                .GetResult();

            if (adminUser == null)
            {
                adminUser = new Tenant
                {
                    UserName = "Admin@Flight",
                    Email = "Admin@Flight.Com",
                    EmailConfirmed = true
                };

                _userManager
                    .CreateAsync(adminUser, "Admin@123")
                    .GetAwaiter()
                    .GetResult();

                _userManager
                    .AddToRoleAsync(adminUser, SD.ADMIN_ROLE)
                    .GetAwaiter()
                    .GetResult();
            }


            // =========================
            // 3. Seed Airports
            // =========================

            if (!_context.Airports.Any())
            {
                var airports = new List<Airport>
                {
                    new Airport
                    {
                        IataCode = "CAI",
                        IcaoCode = "HECA",
                        Name = "Cairo International Airport",
                        City = "Cairo",
                        Country = "Egypt",
                        CountryCode = "EG",
                        Timezone = "Africa/Cairo",
                        Latitude = 30.1219m,
                        Longitude = 31.4056m,
                        IsActive = true
                    },

                    new Airport
                    {
                        IataCode = "LHR",
                        IcaoCode = "EGLL",
                        Name = "London Heathrow Airport",
                        City = "London",
                        Country = "United Kingdom",
                        CountryCode = "GB",
                        Timezone = "Europe/London",
                        Latitude = 51.4700m,
                        Longitude = -0.4543m,
                        IsActive = true
                    },

                    new Airport
                    {
                        IataCode = "DXB",
                        IcaoCode = "OMDB",
                        Name = "Dubai International Airport",
                        City = "Dubai",
                        Country = "United Arab Emirates",
                        CountryCode = "AE",
                        Timezone = "Asia/Dubai",
                        Latitude = 25.2532m,
                        Longitude = 55.3657m,
                        IsActive = true
                    },

                    new Airport
                    {
                        IataCode = "JFK",
                        IcaoCode = "KJFK",
                        Name = "John F. Kennedy International Airport",
                        City = "New York",
                        Country = "United States",
                        CountryCode = "US",
                        Timezone = "America/New_York",
                        Latitude = 40.6413m,
                        Longitude = -73.7781m,
                        IsActive = true
                    }
                };

                _context.Airports.AddRange(airports);
                _context.SaveChanges();
            }


            // =========================
            // 4. Seed Aircrafts
            // =========================

            if (!_context.Aircrafts.Any())
            {
                var aircrafts = new List<Aircraft>
                {
                    new Aircraft
                    {
                        Model = "Boeing 737-800",
                        RegistrationCode = "SU-GAA",
                        Rows = 30,
                        SeatsPerRow = 6,
                        Img = "default.jpg",
                        MaxRangeKm = 5436,
                        Manufacturer = "Boeing",
                        ManufactureDate = new DateTime(2020, 5, 10),
                        IsActive = true
                    },

                    new Aircraft
                    {
                        Model = "Airbus A320",
                        RegistrationCode = "SU-GAB",
                        Rows = 31,
                        SeatsPerRow = 6,
                        Img = "default.jpg",
                        MaxRangeKm = 6100,
                        Manufacturer = "Airbus",
                        ManufactureDate = new DateTime(2021, 3, 15),
                        IsActive = true
                    },

                    new Aircraft
                    {
                        Model = "Boeing 787-9 Dreamliner",
                        RegistrationCode = "SU-GAC",
                        Rows = 42,
                        SeatsPerRow = 9,
                        Img = "default.jpg",
                        MaxRangeKm = 14140,
                        Manufacturer = "Boeing",
                        ManufactureDate = new DateTime(2022, 8, 20),
                        IsActive = true
                    }
                };

                _context.Aircrafts.AddRange(aircrafts);
                _context.SaveChanges();
            }


            // =========================
            // 5. Seed Seats
            // =========================

            if (!_context.Seats.Any())
            {
                var aircrafts = _context.Aircrafts.ToList();

                foreach (var aircraft in aircrafts)
                {
                    var seats = new List<Seat>();
                    int seatNum = 1;

                    for (int i = 0; i < aircraft.Rows; i++)
                    {
                        for (int j = 0; j < aircraft.SeatsPerRow; j++)
                        {
                            bool isWindow = (j == 0 || j == aircraft.SeatsPerRow - 1);
                            bool isAisle = (j == 2 || j == aircraft.SeatsPerRow - 3);

                            int row = j + 1;
                            string column = ((char)('A' + j)).ToString();

                            int third = aircraft.Rows / 3;
                            string zone;
                            SeatClass seatClass;
                            if (i <= third)
                            {
                                zone = "First Class";
                                seatClass = SeatClass.First;
                            }
                            else if (i <= 2 * third)
                            {
                                zone = "Bussiness";
                                seatClass = SeatClass.Business;
                            }
                            else
                            {
                                zone = "Economy";
                                seatClass = SeatClass.Economy;
                            }

                            var seat = new Seat
                            {
                                AircraftId = aircraft.Id,
                                SeatNumber = seatNum.ToString(),
                                Row = row,
                                Column = column,
                                Class = seatClass,
                                Zone = zone,
                                IsExitRow = (i + 1) == 15 || (i + 1) == 16,
                                IsWindow = isWindow,
                                IsAisle = isAisle
                            };

                            seats.Add(seat);
                            seatNum++;
                        }
                    }

                    _context.Seats.AddRange(seats);
                }

                _context.SaveChanges();
            }
        }
    }
}