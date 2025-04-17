using DiamondPharma.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace DiamondPharma.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Admin/PendingPharmacies
        public async Task<IActionResult> PendingPharmacies()
        {
            var pending = await _context.Pharmacies.Where(p => !p.IsApproved).ToListAsync();
            return View(pending);
        }

        // POST: /Admin/Admin/ApprovePharmacy/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApprovePharmacy(int id)
        {
            Console.WriteLine($"Approval attempt for pharmacy ID: {id}");
            var pharmacy = await _context.Pharmacies.FindAsync(id);
            
            if (pharmacy == null)
            {
                Console.WriteLine("Pharmacy not found");
                TempData["Error"] = "Pharmacy not found";
            }
            else
            {
                Console.WriteLine($"Approving pharmacy: {pharmacy.Name}");
                pharmacy.IsApproved = true;
                try
                {
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Approval successful");
                    TempData["Message"] = $"Pharmacy {pharmacy.Name} approved successfully!";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Approval failed: {ex.Message}");
                    TempData["Error"] = $"Approval failed: {ex.Message}";
                }
            }
            return RedirectToAction(nameof(PendingPharmacies), "Admin", new { area = "Admin" });
        }

        // POST: /Admin/Admin/RejectPharmacy/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectPharmacy(int id)
        {
            var pharmacy = await _context.Pharmacies.FindAsync(id);
            if (pharmacy != null)
            {
                _context.Pharmacies.Remove(pharmacy);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Pharmacy rejected successfully!";
            }
            return RedirectToAction(nameof(PendingPharmacies), "Admin", new { area = "Admin" });
        }
    }
}
