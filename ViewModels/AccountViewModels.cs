using System.ComponentModel.DataAnnotations;

namespace SportsBookingApp.ViewModels;

public class RegisterViewModel
{
    [Required, StringLength(100), Display(Name = "Full Name")]
    public string FullName { get; set; }

    [Required, EmailAddress, Display(Name = "Email Address")]
    public string Email { get; set; }

    [Required, StringLength(100, MinimumLength = 6), DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password), Compare(nameof(Password)), Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; }

    [Phone]
    public string Phone { get; set; }

    [StringLength(200)]
    public string Address { get; set; }

    public List<int> SelectedSportIds { get; set; } = new();
}

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, DataType(DataType.Password)]
    public string Password { get; set; }
}