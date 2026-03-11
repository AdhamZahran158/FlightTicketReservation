using FlightTicketReservation.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlightTicketReservation.DbAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Airport> airports { get; set; }
        public DbSet<Aircraft> aircrafts { get; set; }
        public DbSet<Baggage> baggages { get; set; }
        public DbSet<Booking> bookings { get; set; }
        public DbSet<Flight> flights { get; set; }
        public DbSet<Payment> payments { get; set; }
        public DbSet<Reward> rewards { get; set; }
        public DbSet<Seat> seats { get; set; }
        public DbSet<Ticket> ticket { get; set; }
        public DbSet<Trip> trips { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Airport>().HasMany(a => a.DepartingFlights).WithOne(f => f.DepAirport).HasForeignKey(f=>f.DepAirportId);
            builder.Entity<Airport>().HasMany(a => a.ArrivingFlights).WithOne(f => f.ArrAirport).HasForeignKey(a=>a.ArrAirportId);
        }
    }
}
