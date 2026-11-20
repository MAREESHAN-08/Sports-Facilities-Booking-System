using SportsBookingApp.Models;
using System.ComponentModel.DataAnnotations;

namespace SportsBookingApp.ViewModels;

public class ReviewCreateViewModel
{
    public int FacilityID { get; set; }
    public List<Facility> BookedFacilities { get; set; } = new();

    [Required, Range(1, 5, ErrorMessage = "Please select a star rating between 1 and 5.")]
    public int Rating { get; set; }

    [StringLength(500)]
    public string Comments { get; set; }
}

public class HomeStats
{
    public int Members { get; set; }
    public int Facilities { get; set; }
    public int Bookings { get; set; }
}