using Microsoft.Data.SqlClient;
using SportsBookingApp.Models;

namespace SportsBookingApp.Data;

public class ReviewRepository
{
    private readonly string _cs;
    public ReviewRepository(IConfiguration config)
        => _cs = config.GetConnectionString("DefaultConnection");

    public void Add(Review rv)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(@"
INSERT INTO Review (MemberID, FacilityID, Rating, Comments)
VALUES (@m, @f, @r, @c)", con);
        cmd.Parameters.AddWithValue("@m", rv.MemberID);
        cmd.Parameters.AddWithValue("@f", rv.FacilityID);
        cmd.Parameters.AddWithValue("@r", rv.Rating);
        cmd.Parameters.AddWithValue("@c", (object)rv.Comments ?? DBNull.Value);
        con.Open();
        cmd.ExecuteNonQuery();
    }

    public List<Review> GetByFacility(int facilityId)
    {
        var list = new List<Review>();
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(Select + " WHERE rv.FacilityID = @f ORDER BY rv.ReviewDate DESC, rv.ReviewID DESC", con);
        cmd.Parameters.AddWithValue("@f", facilityId);
        con.Open();
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(Map(r));
        return list;
    }

    public List<Review> GetLatest(int n)
    {
        var list = new List<Review>();
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(Select + " ORDER BY rv.ReviewDate DESC, rv.ReviewID DESC OFFSET 0 ROWS FETCH NEXT @n ROWS ONLY", con);
        cmd.Parameters.AddWithValue("@n", n);
        con.Open();
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(Map(r));
        return list;
    }

    public bool HasBooking(int memberId, int facilityId)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(@"
SELECT COUNT(*) FROM Booking
WHERE MemberID = @m AND FacilityID = @f AND Status <> 'Cancelled'", con);
        cmd.Parameters.AddWithValue("@m", memberId);
        cmd.Parameters.AddWithValue("@f", facilityId);
        con.Open();
        return (int)cmd.ExecuteScalar() > 0;
    }

    public List<Facility> GetFacilitiesBookedBy(int memberId)
    {
        var list = new List<Facility>();
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(@"
SELECT DISTINCT f.FacilityID, f.FacilityName
FROM Booking b
JOIN Facility f ON f.FacilityID = b.FacilityID
WHERE b.MemberID = @m AND b.Status <> 'Cancelled'
ORDER BY f.FacilityName", con);
        cmd.Parameters.AddWithValue("@m", memberId);
        con.Open();
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Facility { FacilityID = (int)r["FacilityID"], FacilityName = (string)r["FacilityName"] });
        }
        return list;
    }

    private const string Select = @"
SELECT rv.ReviewID, rv.MemberID, rv.FacilityID, rv.Rating, rv.Comments, rv.ReviewDate,
       m.FullName AS MemberName, f.FacilityName
FROM Review rv
JOIN Member m ON m.MemberID = rv.MemberID
JOIN Facility f ON f.FacilityID = rv.FacilityID";

    private static Review Map(SqlDataReader r) => new()
    {
        ReviewID = (int)r["ReviewID"],
        MemberID = (int)r["MemberID"],
        FacilityID = (int)r["FacilityID"],
        Rating = (int)r["Rating"],
        Comments = r["Comments"] == DBNull.Value ? null : (string)r["Comments"],
        ReviewDate = (DateTime)r["ReviewDate"],
        MemberName = (string)r["MemberName"],
        FacilityName = (string)r["FacilityName"]
    };
}