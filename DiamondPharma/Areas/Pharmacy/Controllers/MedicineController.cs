using DiamondPharma.Data;
using DiamondPharma.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiamondPharma.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    [Authorize]
    public class MedicineController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MedicineController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        // GET: /Pharmacy/Medicine
        public async Task<IActionResult> Index()
        {
            var pharmacy = await GetCurrentPharmacyAsync();
            var medicines = await _context.Medicines
                .Where(m => m.PharmacyId == pharmacy.Id)
                .ToListAsync();
            return View(medicines);
        }

        // GET: /Pharmacy/Medicine/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Pharmacy/Medicine/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medicine medicine)
        {
            if (ModelState.IsValid)
            {
                var pharmacy = await GetCurrentPharmacyAsync();
                medicine.PharmacyId = pharmacy.Id;
                _context.Add(medicine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(medicine);
        }

        // GET: /Pharmacy/Medicine/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null) return NotFound();

            // Verify pharmacy owns this medicine
            var pharmacy = await GetCurrentPharmacyAsync();
            if (medicine.PharmacyId != pharmacy.Id) return Forbid();

            return View(medicine);
        }

        // POST: /Pharmacy/Medicine/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Medicine medicine)
        {
            if (id != medicine.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Ensure ownership
                    var existing = await _context.Medicines.AsNoTracking()
                        .FirstOrDefaultAsync(m => m.Id == id);
                    var pharmacy = await GetCurrentPharmacyAsync();
                    
                    if (existing?.PharmacyId != pharmacy.Id) return Forbid();
                    
                    medicine.PharmacyId = pharmacy.Id; // Prevent tampering
                    _context.Update(medicine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicineExists(medicine.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(medicine);
        }

        // GET: /Pharmacy/Medicine/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null) return NotFound();
            var pharmacy = await GetCurrentPharmacyAsync();
            if (medicine.PharmacyId != pharmacy.Id) return Forbid();
            return View(medicine);
        }

        // GET: /Pharmacy/Medicine/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null) return NotFound();
            var pharmacy = await GetCurrentPharmacyAsync();
            if (medicine.PharmacyId != pharmacy.Id) return Forbid();
            return View(medicine);
        }

        // POST: /Pharmacy/Medicine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            var pharmacy = await GetCurrentPharmacyAsync();
            if (medicine == null || medicine.PharmacyId != pharmacy.Id) return Forbid();
            _context.Medicines.Remove(medicine);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicineExists(int id)
        {
            return _context.Medicines.Any(e => e.Id == id);
        }

        private async Task<Models.Pharmacy> GetCurrentPharmacyAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return await _context.Pharmacies
                .FirstOrDefaultAsync(p => p.Email == user.Email);
        }
    }
}
