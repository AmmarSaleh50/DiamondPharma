using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiamondPharma.Data;
using DiamondPharma.Models;

namespace DiamondPharma.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CatalogMedicineController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CatalogMedicineController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.CatalogMedicines.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CatalogMedicine medicine)
        {
            if (ModelState.IsValid)
            {
                _context.CatalogMedicines.Add(medicine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(medicine);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var med = await _context.CatalogMedicines.FindAsync(id);
            if (med == null) return NotFound();
            return View(med);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CatalogMedicine medicine)
        {
            if (id != medicine.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(medicine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(medicine);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var med = await _context.CatalogMedicines.FindAsync(id);
            if (med == null) return NotFound();
            return View(med);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var med = await _context.CatalogMedicines.FindAsync(id);
            if (med != null)
            {
                _context.CatalogMedicines.Remove(med);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
