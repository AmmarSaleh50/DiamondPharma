using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DiamondPharma.Models;
using Microsoft.AspNetCore.Authorization;

namespace DiamondPharma.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Register", "Pharmacy", new { area = "Pharmacy" });
        }
        // Optionally, redirect authenticated users to their dashboard or pharmacy home
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "CatalogMedicine", new { area = "Admin" });
        }
        else
        {
            return RedirectToAction("Index", "Pharmacy", new { area = "Pharmacy" });
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
