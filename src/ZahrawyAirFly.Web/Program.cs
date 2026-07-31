using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;
using ZahrawyAirFly.Domain.Entities;
using ZahrawyAirFly.Domain.Interfaces;
using ZahrawyAirFly.Infrastructure.Data;
using ZahrawyAirFly.Infrastructure.Repositories;
using ZahrawyAirFly.Infrastructure.Utilities;
using ZahrawyAirFly.Web.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<Tenant, IdentityRole>(confi =>
{
    confi.User.RequireUniqueEmail = true;
    confi.Password.RequiredLength = 8;
    confi.Password.RequireNonAlphanumeric = false;
    confi.Lockout.MaxFailedAccessAttempts = 7;
    confi.SignIn.RequireConfirmedEmail = false;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRepository<Otp>, Repository<Otp>>();
builder.Services.AddScoped<IRepository<Aircraft>, Repository<Aircraft>>();
builder.Services.AddScoped<IRepository<Airport>, Repository<Airport>>();
builder.Services.AddScoped<IRepository<Seat>, Repository<Seat>>();
builder.Services.AddScoped<IRepository<Booking>, Repository<Booking>>();
builder.Services.AddScoped<IRepository<Flight>, Repository<Flight>>();
builder.Services.AddScoped<IRepository<FlightSeat>, Repository<FlightSeat>>();
builder.Services.AddScoped<IRepository<Payment>, Repository<Payment>>();
builder.Services.AddScoped<IRepository<ZahrawyAirFly.Domain.Entities.Discount>, Repository<ZahrawyAirFly.Domain.Entities.Discount>>();
builder.Services.AddScoped<IRepository<BookingLog>, Repository<BookingLog>>();

builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IBookingRepository,  BookingRepository>();

builder.Services.AddScoped<IDBInitialization, DBIntialization>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
StripeConfiguration.ApiKey = builder.Configuration["StripeSettings:SecretKey"];


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider
        .GetRequiredService<IDBInitialization>();

    dbInitializer.Initialize();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapStaticAssets();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=User}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
