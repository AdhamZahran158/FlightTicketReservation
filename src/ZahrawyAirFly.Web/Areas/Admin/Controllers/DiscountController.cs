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
    public class DiscountController : Controller
    {
        private readonly IRepository<Discount> _discountRepository;

        public DiscountController(IRepository<Discount> discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<IActionResult> Index()
        {
            var discounts = await _discountRepository.GetAllAsync();
            return View(discounts);
        }

        [HttpGet]
        public IActionResult AddDiscount()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddDiscount(Discount discount)
        {
            await _discountRepository.AddAsync(discount);
            await _discountRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateDiscount(string id)
        {
            var discount = await _discountRepository.GetOneAsync(d=>d.Id  == id);
            if (discount is null)
            {
                return NotFound();
            }
            return View(discount);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDiscount(Discount model)
        {
            var discount = await _discountRepository.GetOneAsync(d => d.Id == model.Id);

            if (discount == null)
                return NotFound();

            discount.Code = model.Code;
            discount.Description = model.Description;
            discount.Value = model.Value;
            discount.IsPercentage = model.IsPercentage;
            discount.MinBookingAmount = model.MinBookingAmount;
            discount.ValidFrom = model.ValidFrom;
            discount.ValidUntil = model.ValidUntil;
            discount.MaxUses = model.MaxUses;
            discount.IsActive = model.IsActive;

            await _discountRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteDiscount(string id)
        {
            var discount = await _discountRepository.GetOneAsync(d=>d.Id == id);
            if (discount is null)
            {
                return NotFound();
            }
            _discountRepository.Delete(discount);
            await _discountRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
