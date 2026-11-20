namespace SportsBookingApp.Models;

public class Booking
{
    public int BookingID { get; set; }
    public int MemberID { get; set; }
    public int FacilityID { get; set; }
    public DateTime BookingDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal? TotalPrice { get; set; }
    public string Status { get; set; }
    public DateTime CreatedOn { get; set; }

    // Joined columns (for views / PDF)
    public string MemberName { get; set; }
    public string MemberEmail { get; set; }
    public string FacilityName { get; set; }
    public string Location { get; set; }
    public decimal HourlyRate { get; set; }
    public string TypeName { get; set; }

    public string Reference => $"SB-{BookingID:D6}";
    public double Hours => (EndTime - StartTime).TotalHours;
}