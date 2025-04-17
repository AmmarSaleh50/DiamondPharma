using DiamondPharma.Data;
using DiamondPharma.Models;
using DiamondPharma.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DiamondPharma.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    [Authorize]
    public class PharmacyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _currentPharmacyEmail;

        public PharmacyController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _currentPharmacyEmail = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
        }

        // GET: /Pharmacy/Pharmacy/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var pharmacy = await GetCurrentPharmacyAsync();
            if (pharmacy == null || !pharmacy.IsApproved)
                return RedirectToAction("AccessDenied", "Home");

            return View(pharmacy);
        }

        // GET: /Pharmacy/Pharmacy/Profile
        public async Task<IActionResult> Profile()
        {
            var pharmacy = await GetCurrentPharmacyAsync();
            return View(pharmacy);
        }

        // POST: /Pharmacy/Pharmacy/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(Models.Pharmacy model)
        {
            if (ModelState.IsValid)
            {
                var pharmacy = await GetCurrentPharmacyAsync();
                pharmacy.Name = model.Name;
                pharmacy.Address = model.Address;
                pharmacy.Phone = model.Phone;
                await _context.SaveChangesAsync();
                TempData["Message"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Dashboard));
            }
            return View(model);
        }

        // GET: /Pharmacy/Pharmacy/Index
        public async Task<IActionResult> Index()
        {
            var medicines = await _context.CatalogMedicines
                .Where(m => m.IsActive && m.Stock > 0)
                .ToListAsync();
            return View(medicines);
        }

        // GET: /Pharmacy/Pharmacy/Cart
        public IActionResult Cart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var medicines = _context.CatalogMedicines.ToList();
            var cartView = cart.Select(c => new DiamondPharma.Models.CartViewModel {
                Medicine = medicines.FirstOrDefault(m => m.Id == c.MedicineId),
                Quantity = c.Quantity
            }).ToList();
            return View(cartView);
        }

        private async Task<Models.Pharmacy?> GetCurrentPharmacyAsync()
        {
            return await _context.Pharmacies
                .FirstOrDefaultAsync(p => p.Email == _currentPharmacyEmail);
        }
    }
}
