using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Linq.Expressions;
using ZahrawyAirFly.Domain.Entities;
using ZahrawyAirFly.Domain.Enums;
using ZahrawyAirFly.Domain.Interfaces;
using ZahrawyAirFly.Infrastructure.Utilities;

namespace ZahrawyAirFly.Web.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = SD.USER_ROLE)]
    public class BookingController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRepository<Flight> _flightRepository;
        private readonly IRepository<FlightSeat> _flightSeatRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IRepository<BookingLog> _bookingLogRepository;
        private readonly UserManager<Tenant> _userManager;

        public BookingController(
            IBookingRepository bookingRepository,
            IRepository<Flight> flightRepository,
            IRepository<FlightSeat> flightSeatRepository,
            IRepository<Payment> paymentRepository,
            IRepository<BookingLog> bookingLogRepository,
            UserManager<Tenant> userManager)
        {
            _bookingRepository = bookingRepository;
            _flightRepository = flightRepository;
            _flightSeatRepository = flightSeatRepository;
            _paymentRepository = paymentRepository;
            _bookingLogRepository = bookingLogRepository;
            _userManager = userManager;
        }

        // NOT USED
        [HttpGet]
        public async Task<IActionResult> Create(string flightId)
        {
            var flight = await _flightRepository.GetOneAsync(f => f.Id == flightId);

            if (flight == null)
                return NotFound();

            return View(flight);
        }


        [HttpPost]
        public async Task<IActionResult> Create(
    string flightId,
    string[] selectedSeats,
    string passengerName,
    string passportNumber)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            var flight = await _flightRepository.GetOneAsync(f => f.Id == flightId);

            if (flight == null)
                return NotFound();

            decimal subtotal = 0m;
            var flightSeats = new List<FlightSeat>();

            foreach (var seatId in selectedSeats)
            {
                var seat = await _flightSeatRepository.GetOneAsync(
                    s => s.Id == seatId,
                    new Expression<Func<FlightSeat, object>>[]
                    {
                s => s.Seat
                    });

                if (seat == null || seat.Status != SeatStatus.Available)
                    continue;

                flightSeats.Add(seat);

                switch (seat.Seat.Class)
                {
                    case SeatClass.Economy:
                        subtotal += flight.BasePriceEconomy;
                        break;

                    case SeatClass.Business:
                        subtotal += flight.BasePriceBusiness;
                        break;

                    case SeatClass.First:
                        subtotal += flight.BasePriceFirst;
                        break;
                }
            }

            decimal tax = subtotal * 0.14m;
            decimal serviceFee = 50m;
            decimal discount = 0m;
            decimal total = subtotal + tax + serviceFee - discount;

            var booking = new Booking
            {
                BookingRef = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                EncryptedRef = Guid.NewGuid().ToString(),

                FlightId = flight.Id,
                TenantId = user.Id,

                Status = BookingStatus.Pending,

                SubTotal = subtotal,
                TaxAmount = tax,
                FeeAmount = serviceFee,
                DiscountAmount = discount,
                TotalAmount = total,

                BaggageKg = flight.FreeBaggageKg,
                AgreedToTerms = true,
                BookedAt = DateTime.UtcNow
            };

            await _bookingRepository.AddAsync(booking);

            // Save first so Booking.Id is generated
            await _bookingRepository.CommitAsync();

            foreach (var seat in flightSeats)
            {
                // Lock the seat until payment succeeds
                seat.Status = SeatStatus.Locked;
                _flightSeatRepository.Update(seat);

                booking.BookingSeats.Add(new BookingSeat
                {
                    BookingId = booking.Id,
                    FlightSeatId = seat.Id,
                    PassengerName = user.Name,
                    PassportNumber = user.PassportNumber
                });
            }

            await _bookingLogRepository.AddAsync(new BookingLog
            {
                BookingId = booking.Id,
                Action = "Booking Created",
                Details = "Waiting for payment",
                PerformedBy = user.UserName,
                Timestamp = DateTime.UtcNow,
            });

            await _bookingRepository.CommitAsync();

            return RedirectToAction(nameof(Checkout), new { id = booking.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Checkout(string id)
        {
            var booking = await _bookingRepository.GetBookingWithDetailsAsync(id);

            if (booking == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            if (booking.TenantId != userId)
                return Unauthorized();

            return View(booking);
        }

        // Not used
        [HttpPost]
        public async Task<IActionResult> Confirm(string id)
        {
            var booking = await _bookingRepository.GetBookingWithDetailsAsync(id);

            if (booking == null)
                return NotFound();

            booking.Status = BookingStatus.Confirmed;
            _bookingRepository.Update(booking);

            foreach (var item in booking.BookingSeats)
            {
                item.FlightSeat.Status = SeatStatus.Booked;
            }

            await _bookingLogRepository.AddAsync(new BookingLog
            {
                BookingId = booking.Id,
                Action = "Payment Success",
                Details = "Booking confirmed successfully",
                PerformedBy = User.Identity.Name,
                Timestamp = DateTime.UtcNow
            });

            await _bookingRepository.CommitAsync();
            TempData["success"] = "Booked suceesfully";

            return RedirectToAction(nameof(MyBookings));
        }


        // NOT USED
        [HttpGet]
        public async Task<IActionResult> Success(string refNo)
        {
            var booking = await _bookingRepository.GetOneAsync(
                b => b.BookingRef == refNo,
                includes: new System.Linq.Expressions.Expression<Func<Booking, object>>[]
                {
                    x => x.Flight
                });

            if (booking == null)
                return NotFound();

            return View(booking);
        }

   
        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            var bookings = await _bookingRepository.GetUserBookingsWithDetailsAsync(user.Id);
            return View(bookings);
        }


        public async Task<IActionResult> Cancel(string id)
        {
            var booking = await _bookingRepository.GetBookingWithDetailsAsync(id);

            if (booking == null)
                return NotFound();

            if (booking.Flight?.DepartureUtc <= DateTime.UtcNow)
            {
                TempData["error"] = "You cannot cancel a past flight.";
                return RedirectToAction(nameof(MyBookings));
            }

            if (booking.Status == BookingStatus.Cancelled)
            {
                TempData["warning"] = "This booking is already cancelled.";
                return RedirectToAction(nameof(MyBookings));
            }

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = DateTime.UtcNow;

            foreach (var bs in booking.BookingSeats)
            {
                if (bs.FlightSeat != null && bs.FlightSeat.Status == SeatStatus.Booked)
                {
                    bs.FlightSeat.Status = SeatStatus.Available;
                    _flightSeatRepository.Update(bs.FlightSeat);
                }
            }

            _bookingRepository.Update(booking);
            await _bookingRepository.CommitAsync();

            TempData["success"] = "Booking cancelled successfully.";
            return RedirectToAction(nameof(MyBookings));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession(string id)
        {
            var booking = await _bookingRepository.GetBookingWithDetailsAsync(id);

            if (booking == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            if (booking.TenantId != userId)
                return Unauthorized();

            if (booking.Status != BookingStatus.Pending)
            {
                TempData["error"] = "This booking is not in a payable state.";
                return RedirectToAction(nameof(Checkout), new { id = booking.Id });
            }

            var domain = $"{Request.Scheme}://{Request.Host}";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(booking.TotalAmount * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"Booking {booking.BookingRef}"
                    }
                },
                Quantity = 1
            }
        },
                Mode = "payment",
                SuccessUrl = $"{domain}/User/Booking/PaymentReturn?bookingId={booking.Id}&session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{domain}/User/Booking/Checkout?id={booking.Id}",
                Metadata = new Dictionary<string, string>
        {
            { "bookingId", booking.Id.ToString() }
        }
            };

            var service = new SessionService();
            var session = service.Create(options);

            await _paymentRepository.AddAsync(new Payment
            {
                BookingId = booking.Id,
                Amount = booking.TotalAmount,
                Currency = "USD",
                Method = PaymentMethod.CreditCard,
                TransactionRef = session.Id,   // مؤقتًا، هنستبدلها بـ PaymentIntent بعد النجاح
                Status = PaymentStatus.Pending,
                PaidAt = null
            });
            await _bookingRepository.CommitAsync();

            return Redirect(session.Url);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentReturn(string bookingId, string session_id)
        {
            if (string.IsNullOrWhiteSpace(bookingId) ||
                string.IsNullOrWhiteSpace(session_id))
            {
                return BadRequest();
            }

            var booking = await _bookingRepository.GetBookingWithDetailsAsync(bookingId);

            if (booking == null)
                return NotFound();

            try
            {
                var sessionService = new SessionService();
                var session = await sessionService.GetAsync(session_id);

                if (session.Status == "complete" &&
                    session.PaymentStatus == "paid" &&
                    booking.Status == BookingStatus.Pending)
                {
                    booking.Status = BookingStatus.Confirmed;

                    foreach (var bookingSeat in booking.BookingSeats)
                    {
                        bookingSeat.FlightSeat.Status = SeatStatus.Booked;
                    }

                    var payment = await _paymentRepository.GetOneAsync(
                        p => p.TransactionRef == session.Id);

                    if (payment != null)
                    {
                        payment.Status = PaymentStatus.Completed;
                        payment.PaidAt = DateTime.UtcNow;
                    }

                    await _bookingLogRepository.AddAsync(new BookingLog
                    {
                        BookingId = booking.Id,
                        Action = "Payment Success",
                        Details = "Booking confirmed successfully",
                        PerformedBy = User.Identity?.Name ?? "Stripe",
                        Timestamp = DateTime.UtcNow
                    });

                    await _bookingRepository.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> CheckStatus(string id)
        {
            var booking = await _bookingRepository.GetOneAsync(b => b.Id == id);
            if (booking == null)
                return NotFound();

            return Json(new { status = booking.Status.ToString() });
        }
    }
}
