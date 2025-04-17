using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiamondPharma.Data;
using DiamondPharma.Models;

namespace DiamondPharma.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<Models.Pharmacy> GetCurrentPharmacyAsync()
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            return await _context.Pharmacies.FirstOrDefaultAsync(p => p.Email == email);
        }

        // GET: /Pharmacy/Order
        public async Task<IActionResult> Index()
        {
            var pharmacy = await GetCurrentPharmacyAsync();
            var orders = await _context.Orders
                .Where(o => o.PharmacyId == pharmacy.Id)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Medicine)
                .ToListAsync();
            return View(orders);
        }

        // GET: /Pharmacy/Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var pharmacy = await GetCurrentPharmacyAsync();
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Medicine)
                .FirstOrDefaultAsync(o => o.Id == id && o.PharmacyId == pharmacy.Id);
            if (order == null) return NotFound();
            return View(order);
        }
    }
}
