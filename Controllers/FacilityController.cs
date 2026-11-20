using Microsoft.AspNetCore.Mvc;
using SportsBookingApp.Data;
using SportsBookingApp.Filters;
using SportsBookingApp.Models;
using SportsBookingApp.ViewModels;

namespace SportsBookingApp.Controllers;

public class FacilityController : Controller
{
    private readonly FacilityRepository _fac;
    private readonly ReviewRepository _rev;

    public FacilityController(FacilityRepository fac, ReviewRepository rev)
    {
        _fac = fac; _rev = rev;
    }

    // ================= GUESTS: RESTRICTED SEARCH =================
    // Guests can browse facility types & availability broadly (no rates, no booking)
    public IActionResult Browse(string location = null, int? typeId = null)
    {
        ViewBag.Types = _fac.GetTypes();
        ViewBag.Location = location;
        ViewBag.TypeId = typeId;
        ViewBag.Results = _fac.Search(typeId, location, null, null, null);
        return View();
    }

    // ================= MEMBERS: FULL SEARCH =================
    [MemberOnly]
    public IActionResult Search(FacilitySearchViewModel vm)
    {
        vm.Types = _fac.GetTypes();

        var anyTime = vm.Date.HasValue || vm.StartTime.HasValue || vm.EndTime.HasValue;
        var allTime = vm.Date.HasValue && vm.StartTime.HasValue && vm.EndTime.HasValue;

        if (anyTime && !allTime)
        {
            ModelState.AddModelError(string.Empty,
                "To check live availability, please provide the date, start time and end time together.");
            vm.Results = _fac.Search(vm.FacilityTypeId, vm.Location, null, null, null);
        }
        else if (allTime && vm.EndTime.Value <= vm.StartTime.Value)
        {
            ModelState.AddModelError(string.Empty, "End time must be after the start time.");
            vm.Results = _fac.Search(vm.FacilityTypeId, vm.Location, null, null, null);
        }
        else
        {
            vm.Results = _fac.Search(vm.FacilityTypeId, vm.Location,
                allTime ? vm.Date : null,
                allTime ? vm.StartTime : null,
                allTime ? vm.EndTime : null);
        }

        vm.Searched = true;
        return View(vm);
    }

    // ================= DETAILS (public, limited for guests) =================
    public IActionResult Details(int id)
    {
        var facility = _fac.GetById(id);
        if (facility == null) return NotFound();
        ViewBag.Reviews = _rev.GetByFacility(id);
        return View(facility);
    }
}