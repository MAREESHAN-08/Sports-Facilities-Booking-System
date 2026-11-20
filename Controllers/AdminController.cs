using Microsoft.AspNetCore.Mvc;
using SportsBookingApp.Data;
using SportsBookingApp.Filters;

namespace SportsBookingApp.Controllers;

public class AdminController : Controller
{
    private readonly BookingRepository _book;
    private readonly IConfiguration _config;

    public AdminController(BookingRepository book, IConfiguration config)
    {
        _book = book; _config = config;
    }

    public IActionResult Login()
    {
        if (HttpContext.Session.GetString("Admin") != null)
            return RedirectToAction(nameof(Dashboard));
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Login(string email, string password)
    {
        var cfg = _config.GetSection("Admin");
        if (string.Equals(email, cfg["Email"], StringComparison.OrdinalIgnoreCase)
            && password == cfg["Password"])
        {
            HttpContext.Session.SetString("Admin", "true");
            return RedirectToAction(nameof(Dashboard));
        }
        ModelState.AddModelError(string.Empty, "Invalid admin credentials.");
        return View();
    }

    [AdminOnly]
    public IActionResult Dashboard() => View(_book.GetAll());

    [AdminOnly, HttpPost, ValidateAntiForgeryToken]
    public IActionResult SetStatus(int id, string status)
    {
        if (_book.SetStatus(id, status))
            TempData["Ok"] = $"Booking SB-{id:D6} marked as {status}.";
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}