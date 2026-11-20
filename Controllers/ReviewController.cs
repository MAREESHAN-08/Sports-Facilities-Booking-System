using Microsoft.AspNetCore.Mvc;
using SportsBookingApp.Data;
using SportsBookingApp.Filters;
using SportsBookingApp.Models;
using SportsBookingApp.ViewModels;

namespace SportsBookingApp.Controllers;

public class ReviewController : Controller
{
    private readonly ReviewRepository _rev;
    private readonly FacilityRepository _fac;

    public ReviewController(ReviewRepository rev, FacilityRepository fac)
    {
        _rev = rev; _fac = fac;
    }

    private int MemberId => (int)HttpContext.Session.GetInt32("MemberID");

    // ---------- GUESTS: VIEW ALL REVIEWS ----------
    public IActionResult Latest()
    {
        ViewBag.Reviews = _rev.GetLatest(12);
        return View();
    }

    // ---------- GUESTS: REVIEWS FOR ONE FACILITY ----------
    public IActionResult Facility(int id)
    {
        var facility = _fac.GetById(id);
        if (facility == null) return NotFound();
        ViewBag.Facility = facility;
        ViewBag.Reviews = _rev.GetByFacility(id);
        return View();
    }

    // ---------- MEMBERS: SUBMIT A REVIEW ----------
    [MemberOnly]
    public IActionResult Create(int? facilityId)
    {
        var vm = new ReviewCreateViewModel
        {
            BookedFacilities = _rev.GetFacilitiesBookedBy(MemberId),
            FacilityID = facilityId ?? 0
        };

        if (vm.BookedFacilities.Count == 0)
        {
            TempData["Info"] = "You can review a facility after you have a booking with it. Book a facility first!";
            return RedirectToAction("Search", "Facility");
        }
        return View(vm);
    }

    [MemberOnly, HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(ReviewCreateViewModel vm)
    {
        vm.BookedFacilities = _rev.GetFacilitiesBookedBy(MemberId);

        if (vm.FacilityID <= 0)
            ModelState.AddModelError(nameof(vm.FacilityID), "Please choose a facility.");
        else if (!_rev.HasBooking(MemberId, vm.FacilityID))
            ModelState.AddModelError(nameof(vm.FacilityID),
                "You can only review facilities you have booked.");

        if (!ModelState.IsValid) return View(vm);

        _rev.Add(new Review
        {
            MemberID = MemberId,
            FacilityID = vm.FacilityID,
            Rating = vm.Rating,
            Comments = vm.Comments,
            ReviewDate = DateTime.Today
        });

        TempData["Ok"] = "Thanks for your review! It is now visible to everyone.";
        return RedirectToAction(nameof(Facility), new { id = vm.FacilityID });
    }
}