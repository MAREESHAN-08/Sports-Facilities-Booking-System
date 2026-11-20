namespace SportsBookingApp.Models;

public class Review
{
    public int ReviewID { get; set; }
    public int MemberID { get; set; }
    public int FacilityID { get; set; }
    public int Rating { get; set; }
    public string Comments { get; set; }
    public DateTime ReviewDate { get; set; }

    // Joined
    public string MemberName { get; set; }
    public string FacilityName { get; set; }
}