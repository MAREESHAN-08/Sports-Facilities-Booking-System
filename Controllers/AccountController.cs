using Microsoft.AspNetCore.Mvc;
using SportsBookingApp.Data;
using SportsBookingApp.Filters;
using SportsBookingApp.Models;
using SportsBookingApp.ViewModels;

namespace SportsBookingApp.Controllers;

public class AccountController : Controller
{
    private readonly MemberRepository _members;

    public AccountController(MemberRepository members) => _members = members;

    // ---------- LOGIN ----------
    public IActionResult Login(string returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel vm, string returnUrl = null)
    {
        if (!ModelState.IsValid) return View(vm);

        var member = _members.Authenticate(vm.Email, vm.Password);
        if (member == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password. Please try again.");
            return View(vm);
        }

        HttpContext.Session.SetInt32("MemberID", member.MemberID);
        HttpContext.Session.SetString("MemberName", member.FullName);

        TempData["Ok"] = $"Login successful! Welcome back, {member.FullName}.";

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    // ---------- ⚡ 1-CLICK GUEST LOGIN ----------
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult GuestLogin(string returnUrl = null)
    {
        try
        {
            var guest = _members.GetOrCreateGuestMember();
            HttpContext.Session.SetInt32("MemberID", guest.MemberID);
            HttpContext.Session.SetString("MemberName", guest.FullName);

            TempData["Ok"] = "Login successful! You are signed in as a Guest Member. Enjoy exploring and booking sports venues!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Browse", "Facility");
        }
        catch (Exception)
        {
            TempData["Info"] = "Guest login temporarily unavailable. Please register or sign in with your email.";
            return RedirectToAction(nameof(Login));
        }
    }

    // ---------- LOGOUT ----------
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["Info"] = "You have been logged out. See you soon!";
        return RedirectToAction("Index", "Home");
    }

    // ---------- REGISTER ----------
    public IActionResult Register()
    {
        ViewBag.Sports = _members.GetAllSports();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel vm)
    {
        ViewBag.Sports = _members.GetAllSports();

        if (!ModelState.IsValid) return View(vm);

        if (_members.EmailExists(vm.Email))
        {
            ModelState.AddModelError(nameof(vm.Email),
                "This email is already registered. Try logging in instead.");
            return View(vm);
        }

        int id = _members.Create(vm, PasswordHasher.Hash(vm.Password));

        HttpContext.Session.SetInt32("MemberID", id);
        HttpContext.Session.SetString("MemberName", vm.FullName);

        TempData["Ok"] = $"Registration & login successful! Welcome to ArenaPulse, {vm.FullName}! You can now book facilities.";
        return RedirectToAction("Index", "Home");
    }
}