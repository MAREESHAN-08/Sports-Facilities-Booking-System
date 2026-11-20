using SportsBookingApp.Models;
using System.ComponentModel.DataAnnotations;

namespace SportsBookingApp.ViewModels;

public class BookingCreateViewModel
{
    public int FacilityID { get; set; }
    public Facility Facility { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime? BookingDate { get; set; }

    [Required]
    public TimeSpan? StartTime { get; set; }

    [Required]
    public TimeSpan? EndTime { get; set; }

    public decimal? TotalPrice { get; set; }
}

public class ReceiptViewModel
{
    public int BookingID { get; set; }
    public string Reference { get; set; }
    public string Status { get; set; }
    public string MemberName { get; set; }
    public string MemberEmail { get; set; }
    public string FacilityName { get; set; }
    public string TypeName { get; set; }
    public string Location { get; set; }
    public string ContactPhone { get; set; }
    public DateTime BookingDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public double Hours { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedOn { get; set; }
}