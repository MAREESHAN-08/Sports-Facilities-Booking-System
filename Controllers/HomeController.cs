using Microsoft.AspNetCore.Mvc;
using SportsBookingApp.Data;
using SportsBookingApp.Models;
using SportsBookingApp.ViewModels;

namespace SportsBookingApp.Controllers;

public class HomeController : Controller
{
    private readonly FacilityRepository _fac;
    private readonly ReviewRepository _rev;
    private readonly InquiryRepository _inq;
    private readonly BookingRepository _book;

    public HomeController(FacilityRepository fac, ReviewRepository rev,
                          InquiryRepository inq, BookingRepository book)
    {
        _fac = fac; _rev = rev; _inq = inq; _book = book;
    }

    public IActionResult Index()
    {
        ViewBag.Featured = _fac.GetFeatured(3);
        ViewBag.LatestReviews = _rev.GetLatest(3);
        ViewBag.Stats = _book.GetStats();
        return View();
    }

    public IActionResult About()
    {
        ViewBag.Stats = _book.GetStats();
        ViewBag.Types = _fac.GetTypes();
        ViewBag.Featured = _fac.GetFeatured(4);
        return View();
    }

    public IActionResult Contact() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Contact(Inquiry inq)
    {
        if (!ModelState.IsValid) return View(inq);
        _inq.Add(inq);
        TempData["Ok"] = "Thank you! Your inquiry has been sent to the Sports Council. We will reply to your email soon.";
        return RedirectToAction(nameof(Contact));
    }

    public IActionResult Error()
    {
        ViewData["RequestId"] = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return View();
    }
}