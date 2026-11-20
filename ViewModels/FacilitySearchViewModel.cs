using SportsBookingApp.Models;

namespace SportsBookingApp.ViewModels;

public class FacilitySearchViewModel
{
    public int? FacilityTypeId { get; set; }
    public string Location { get; set; }
    public DateTime? Date { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public bool Searched { get; set; }
    public List<FacilityType> Types { get; set; } = new();
    public List<Facility> Results { get; set; } = new();
}