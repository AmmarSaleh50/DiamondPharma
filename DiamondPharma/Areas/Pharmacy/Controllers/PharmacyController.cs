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

        // POST: /Pharmacy/Pharmacy/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int medicineId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var existing = cart.FirstOrDefault(c => c.MedicineId == medicineId);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem { MedicineId = medicineId, Quantity = quantity });
            }
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Show a success message after adding to cart
            TempData["CartSuccess"] = "Item added to cart!";
            return RedirectToAction("Index");
        }

        // POST: /Pharmacy/Pharmacy/UpdateCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(List<int> Quantities, int? removeIndex)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            if (removeIndex.HasValue && removeIndex.Value >= 0 && removeIndex.Value < cart.Count)
            {
                cart.RemoveAt(removeIndex.Value);
                TempData["CartSuccess"] = "Item removed from cart.";
            }
            else if (Quantities != null && Quantities.Count == cart.Count)
            {
                for (int i = 0; i < cart.Count; i++)
                {
                    cart[i].Quantity = Quantities[i] > 0 ? Quantities[i] : 1;
                }
                TempData["CartSuccess"] = "Cart updated.";
            }
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            // If AJAX, return partial view
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var medicines = _context.CatalogMedicines.ToList();
                var cartView = cart.Select(c => new DiamondPharma.Models.CartViewModel {
                    Medicine = medicines.FirstOrDefault(m => m.Id == c.MedicineId),
                    Quantity = c.Quantity
                }).ToList();
                return PartialView("_CartTablePartial", cartView);
            }
            return RedirectToAction("Cart");
        }

        private async Task<Models.Pharmacy?> GetCurrentPharmacyAsync()
        {
            return await _context.Pharmacies
                .FirstOrDefaultAsync(p => p.Email == _currentPharmacyEmail);
        }
    }
}
