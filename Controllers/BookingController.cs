using Microsoft.AspNetCore.Mvc;
using SportsBookingApp.Data;
using SportsBookingApp.Filters;
using SportsBookingApp.Models;
using SportsBookingApp.Services;
using SportsBookingApp.ViewModels;

namespace SportsBookingApp.Controllers;

public class BookingController : Controller
{
    private readonly BookingRepository _book;
    private readonly FacilityRepository _fac;

    public BookingController(BookingRepository book, FacilityRepository fac)
    {
        _book = book; _fac = fac;
    }

    private int MemberId => (int)HttpContext.Session.GetInt32("MemberID");

    // ---------- BOOK A FACILITY ----------
    [MemberOnly]
    public IActionResult Book(int facilityId)
    {
        var facility = _fac.GetById(facilityId);
        if (facility == null || !facility.IsAvailable) return NotFound();

        return View(new BookingCreateViewModel
        {
            FacilityID = facility.FacilityID,
            Facility = facility
        });
    }

    [MemberOnly, HttpPost, ValidateAntiForgeryToken]
    public IActionResult Book(BookingCreateViewModel vm)
    {
        var facility = _fac.GetById(vm.FacilityID);
        if (facility == null || !facility.IsAvailable) return NotFound();
        vm.Facility = facility;

        if (vm.BookingDate == null || vm.StartTime == null || vm.EndTime == null)
        {
            ModelState.AddModelError(string.Empty, "Please choose the booking date and both times.");
            return View(vm);
        }
        if (vm.EndTime.Value <= vm.StartTime.Value)
        {
            ModelState.AddModelError(string.Empty, "End time must be after the start time.");
            return View(vm);
        }
        if (vm.BookingDate.Value.Date < DateTime.Today)
        {
            ModelState.AddModelError(string.Empty, "You cannot book a date in the past.");
            return View(vm);
        }
        if (vm.BookingDate.Value.Date == DateTime.Today && vm.StartTime.Value <= DateTime.Now.TimeOfDay)
        {
            ModelState.AddModelError(string.Empty, "That start time has already passed for today. Pick a later slot.");
            return View(vm);
        }
        if (!_book.IsSlotFree(facility.FacilityID, vm.BookingDate.Value, vm.StartTime.Value, vm.EndTime.Value))
        {
            ModelState.AddModelError(string.Empty,
                "Sorry, that slot is already booked for this facility. Please choose another time.");
            return View(vm);
        }

        double hours = (vm.EndTime.Value - vm.StartTime.Value).TotalHours;
        decimal price = Math.Round((decimal)hours * facility.HourlyRate, 2);

        int id = _book.Create(new Booking
        {
            MemberID = MemberId,
            FacilityID = facility.FacilityID,
            BookingDate = vm.BookingDate.Value.Date,
            StartTime = vm.StartTime.Value,
            EndTime = vm.EndTime.Value,
            TotalPrice = price
        });

        TempData["Ok"] = $"Booking placed successfully — reference SB-{id:D6}. " +
                         "Download your PDF receipt from My Bookings below!";
        return RedirectToAction(nameof(MyBookings));
    }

    // ---------- MY BOOKINGS ----------
    [MemberOnly]
    public IActionResult MyBookings()
    {
        return View(_book.GetByMember(MemberId));
    }

    // ---------- CANCEL ----------
    [MemberOnly, HttpPost, ValidateAntiForgeryToken]
    public IActionResult Cancel(int id)
    {
        if (_book.Cancel(id, MemberId))
            TempData["Info"] = $"Booking SB-{id:D6} has been cancelled.";
        return RedirectToAction(nameof(MyBookings));
    }

    // ---------- 🧾 PDF RECEIPT DOWNLOAD ----------
    [MemberOnly]
    public IActionResult Receipt(int id)
    {
        var b = _book.GetByIdFull(id);
        if (b == null) return NotFound();

        if (b.MemberID != MemberId)
        {
            TempData["Info"] = "You can only download receipts for your own bookings.";
            return RedirectToAction(nameof(MyBookings));
        }

        var receipt = new ReceiptViewModel
        {
            BookingID = b.BookingID,
            Reference = b.Reference,
            Status = b.Status,
            MemberName = b.MemberName,
            MemberEmail = b.MemberEmail,
            FacilityName = b.FacilityName,
            TypeName = b.TypeName,
            Location = b.Location,
            ContactPhone = _fac.GetById(b.FacilityID)?.ContactPhone,
            BookingDate = b.BookingDate,
            StartTime = b.StartTime,
            EndTime = b.EndTime,
            Hours = b.Hours,
            HourlyRate = b.HourlyRate,
            TotalPrice = b.TotalPrice ?? 0,
            CreatedOn = b.CreatedOn
        };

        byte[] pdf = ReceiptPdfGenerator.Generate(receipt);
        return File(pdf, "application/pdf", $"Receipt_{b.Reference}.pdf");
    }
}