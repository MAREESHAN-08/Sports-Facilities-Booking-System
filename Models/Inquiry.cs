namespace SportsBookingApp.Models;

public class Inquiry
{
    public int InquiryID { get; set; }
    public string GuestName { get; set; }
    public string GuestEmail { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
    public DateTime InquiryDate { get; set; }
    public string Status { get; set; }
}