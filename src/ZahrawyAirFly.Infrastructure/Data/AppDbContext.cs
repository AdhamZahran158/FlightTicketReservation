using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ZahrawyAirFly.Domain.Entities;
using ZahrawyAirFly.Shared.Base;

namespace ZahrawyAirFly.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<Tenant>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Airport> Airports { get; set; }
        public DbSet<Aircraft> Aircrafts { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<FlightSeat> FlightSeats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingSeat> BookingSeats { get; set; }

        public DbSet<Offer> Offers { get; set; }
        public DbSet<FlightOffer> FlightOffers { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<TaxFee> TaxFees { get; set; }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<Policy> Policies { get; set; }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<BookingLog> BookingLogs { get; set; }

        public DbSet<Otp> otps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Tenant>()
                .HasMany<Booking>()
                .WithOne(b => b.Tenant)
                .HasForeignKey(b => b.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tenant>()
                .HasMany<Notification>()
                .WithOne(n => n.Tenant)
                .HasForeignKey(n => n.TenantId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Aircraft>()
                .HasMany(a => a.Seats)
                .WithOne(s => s.Aircraft)
                .HasForeignKey(s => s.AircraftId)
                .OnDelete(DeleteBehavior.Cascade);

           

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Aircraft)
                .WithMany()
                .HasForeignKey(f => f.AircraftId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.OriginAirport)
                .WithMany()
                .HasForeignKey(f => f.OriginAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.DestinationAirport)
                .WithMany()
                .HasForeignKey(f => f.DestinationAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Flight>()
                .HasMany(f => f.FlightSeats)
                .WithOne(fs => fs.Flight)
                .HasForeignKey(fs => fs.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Flight>()
                .HasMany(f => f.Bookings)
                .WithOne(b => b.Flight)
                .HasForeignKey(b => b.FlightId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Flight>()
                .HasMany(f => f.FlightOffers)
                .WithOne(fo => fo.Flight)
                .HasForeignKey(fo => fo.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            

            modelBuilder.Entity<FlightSeat>()
                .HasOne(fs => fs.Seat)
                .WithMany()
                .HasForeignKey(fs => fs.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

           

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Discount)
                .WithMany(d => d.Bookings)
                .HasForeignKey(b => b.DiscountId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.BookingSeats)
                .WithOne(bs => bs.Booking)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Payments)
                .WithOne(p => p.Booking)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.BookingLogs)
                .WithOne(bl => bl.Booking)
                .HasForeignKey(bl => bl.BookingId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<BookingSeat>()
                .HasOne(bs => bs.FlightSeat)
                .WithMany()
                .HasForeignKey(bs => bs.FlightSeatId)
                .OnDelete(DeleteBehavior.Restrict);

     

            modelBuilder.Entity<FlightOffer>()
                .HasOne(fo => fo.Offer)
                .WithMany(o => o.FlightOffers)
                .HasForeignKey(fo => fo.OfferId)
                .OnDelete(DeleteBehavior.Cascade);

       

            modelBuilder.Entity<Tenant>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<Airport>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<Aircraft>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<Seat>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<Flight>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<FlightSeat>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<Booking>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<BookingSeat>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<Offer>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<Discount>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<TaxFee>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<Payment>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<Policy>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<Notification>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<BookingLog>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<FlightOffer>()
                .HasQueryFilter(e => !e.IsDeleted);

            
            modelBuilder.Entity<Tenant>()
                .HasIndex(t => t.Subdomain)
                .IsUnique();

            modelBuilder.Entity<Airport>()
                .HasIndex(a => a.IataCode)
                .IsUnique();

            modelBuilder.Entity<Airport>()
                .HasIndex(a => a.IcaoCode)
                .IsUnique();

            modelBuilder.Entity<Aircraft>()
                .HasIndex(a => a.RegistrationCode)
                .IsUnique();

            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.BookingRef)
                .IsUnique();

            modelBuilder.Entity<Discount>()
                .HasIndex(d => d.Code)
                .IsUnique();

            modelBuilder.Entity<FlightSeat>()
                .HasIndex(fs => new { fs.FlightId, fs.SeatId })
                .IsUnique();

            modelBuilder.Entity<FlightOffer>()
                .HasIndex(fo => new { fo.FlightId, fo.OfferId })
                .IsUnique();

           

            modelBuilder.Entity<Airport>()
                .HasIndex(a => new { a.City, a.Country });


            modelBuilder.Entity<Flight>()
                .HasIndex(f => new { f.OriginAirportId, f.DestinationAirportId });

            modelBuilder.Entity<FlightSeat>()
                .HasIndex(fs => new { fs.FlightId, fs.Status });

            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.TenantId, b.Status });

            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.TenantId, b.BookedAt });

            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.FlightId, b.Status });

            modelBuilder.Entity<Notification>()
                .HasIndex(n => new { n.TenantId, n.IsRead });

            
            modelBuilder.Entity<Airport>()
                .Property(a => a.Latitude)
                .HasPrecision(9, 6);

            modelBuilder.Entity<Airport>()
                .Property(a => a.Longitude)
                .HasPrecision(9, 6);

            modelBuilder.Entity<Flight>()
                .Property(f => f.BasePriceEconomy)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Flight>()
                .Property(f => f.BasePriceBusiness)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Flight>()
                .Property(f => f.BasePriceFirst)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Flight>()
                .Property(f => f.ExtraBaggagePerKg)
                .HasPrecision(18, 2);

            modelBuilder.Entity<FlightSeat>()
                .Property(fs => fs.PriceOverride)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.SubTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.DiscountAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.TaxAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.FeeAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Offer>()
                .Property(o => o.DiscountValue)
                .HasPrecision(18, 2);

            
            modelBuilder.Entity<Discount>()
                .Property(d => d.Value)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Discount>()
                .Property(d => d.MinBookingAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Policy>()
                .Property(p => p.CancelPenaltyPercent)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Policy>()
                .Property(p => p.ModifyFee)
                .HasPrecision(18, 2);

            modelBuilder.Entity<TaxFee>()
                .Property(tf => tf.Value)
                .HasPrecision(18, 2);
        }


        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}