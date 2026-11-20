namespace SportsBookingApp.Models;

public class FacilityType
{
    public int FacilityTypeID { get; set; }
    public string TypeName { get; set; }
    public string Description { get; set; }
}

public class Facility
{
    public int FacilityID { get; set; }
    public string FacilityName { get; set; }
    public int FacilityTypeID { get; set; }
    public string TypeName { get; set; }      // joined from FacilityType
    public string Location { get; set; }
    public int Capacity { get; set; }
    public decimal HourlyRate { get; set; }
    public string ContactPhone { get; set; }
    public string Description { get; set; }
    public bool IsAvailable { get; set; }
    public double? AvgRating { get; set; }    // computed from Review table
    public int ReviewCount { get; set; }
}