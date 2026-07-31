using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using ZahrawyAirFly.Domain.Entities;
using ZahrawyAirFly.Domain.Enums;
using ZahrawyAirFly.Domain.Interfaces;

namespace ZahrawyAirFly.Web.Controllers
{
    [Route("api/stripe-webhook")]
    [ApiController]
    public class StripeWebhookController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IRepository<BookingLog> _bookingLogRepository;
        private readonly IConfiguration _config;

        public StripeWebhookController(
            IBookingRepository bookingRepository,
            IRepository<Payment> paymentRepository,
            IRepository<BookingLog> bookingLogRepository,
            IConfiguration config)
        {
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
            _bookingLogRepository = bookingLogRepository;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var webhookSecret = _config["StripeSettings:WebhookSecret"];

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    webhookSecret
                );
            }
            catch (StripeException e)
            {
                return BadRequest($"Webhook signature verification failed: {e.Message}");
            }

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;
                var bookingId = session.Metadata["bookingId"];

                var booking = await _bookingRepository.GetBookingWithDetailsAsync(bookingId);

                if (booking != null && booking.Status == BookingStatus.Pending)
                {
                    booking.Status = BookingStatus.Confirmed;

                    foreach (var item in booking.BookingSeats)
                        item.FlightSeat.Status = SeatStatus.Booked;

                    var payments = await _paymentRepository.GetAsync(p => p.TransactionRef == session.Id);
                    var payment = payments.FirstOrDefault();

                    if (payment != null)
                    {
                        payment.Status = PaymentStatus.Completed;
                        payment.TransactionRef = session.PaymentIntentId ?? session.Id;
                        payment.PaidAt = DateTime.UtcNow;
                        _paymentRepository.Update(payment);
                    }

                    await _bookingLogRepository.AddAsync(new BookingLog
                    {
                        BookingId = booking.Id,
                        Action = "Payment Success",
                        Details = $"Payment confirmed via Stripe session {session.Id}",
                        PerformedBy = "Stripe Webhook",
                        Timestamp = DateTime.UtcNow
                    });

                    await _bookingRepository.CommitAsync();
                }
            }
            else if (stripeEvent.Type == "checkout.session.expired")
            {
                var session = stripeEvent.Data.Object as Session;
                var payments = await _paymentRepository.GetAsync(p => p.TransactionRef == session.Id);
                var payment = payments.FirstOrDefault();

                if (payment != null)
                {
                    payment.Status = PaymentStatus.Failed;
                    _paymentRepository.Update(payment);
                    await _bookingRepository.CommitAsync();
                }
            }

            return Ok();
        }
    }
}