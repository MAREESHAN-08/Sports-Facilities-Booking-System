using Microsoft.Data.SqlClient;
using SportsBookingApp.Models;
using SportsBookingApp.ViewModels;
using System.Data;

namespace SportsBookingApp.Data;

public class BookingRepository
{
    private readonly string _cs;
    public BookingRepository(IConfiguration config)
        => _cs = config.GetConnectionString("DefaultConnection");

    public bool IsSlotFree(int facilityId, DateTime date, TimeSpan start, TimeSpan end)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(@"
SELECT COUNT(*) FROM Booking
WHERE FacilityID = @f AND Status <> 'Cancelled' AND BookingDate = @d
  AND StartTime < @end AND EndTime > @start", con);
        cmd.Parameters.AddWithValue("@f", facilityId);
        cmd.Parameters.Add("@d", SqlDbType.Date).Value = date.Date;
        cmd.Parameters.Add("@start", SqlDbType.Time).Value = start;
        cmd.Parameters.Add("@end", SqlDbType.Time).Value = end;
        con.Open();
        return (int)cmd.ExecuteScalar() == 0;
    }

    public int Create(Booking b)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(@"
INSERT INTO Booking (MemberID, FacilityID, BookingDate, StartTime, EndTime, TotalPrice, Status)
OUTPUT INSERTED.BookingID
VALUES (@m, @f, @d, @s, @e, @p, 'Pending')", con);
        cmd.Parameters.AddWithValue("@m", b.MemberID);
        cmd.Parameters.AddWithValue("@f", b.FacilityID);
        cmd.Parameters.Add("@d", SqlDbType.Date).Value = b.BookingDate.Date;
        cmd.Parameters.Add("@s", SqlDbType.Time).Value = b.StartTime;
        cmd.Parameters.Add("@e", SqlDbType.Time).Value = b.EndTime;
        cmd.Parameters.AddWithValue("@p", (object)b.TotalPrice ?? DBNull.Value);
        con.Open();
        return (int)cmd.ExecuteScalar();
    }

    public List<Booking> GetByMember(int memberId)
    {
        var list = new List<Booking>();
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(BaseSelect + " WHERE b.MemberID = @m ORDER BY b.BookingDate DESC, b.StartTime", con);
        cmd.Parameters.AddWithValue("@m", memberId);
        con.Open();
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(Map(r));
        return list;
    }

    public List<Booking> GetAll()
    {
        var list = new List<Booking>();
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(BaseSelect + " ORDER BY b.CreatedOn DESC", con);
        con.Open();
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(Map(r));
        return list;
    }

    public Booking GetByIdFull(int id)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(BaseSelect + " WHERE b.BookingID = @id", con);
        cmd.Parameters.AddWithValue("@id", id);
        con.Open();
        using var r = cmd.ExecuteReader();
        return r.Read() ? Map(r) : null;
    }

    public bool Cancel(int bookingId, int memberId)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(
            "UPDATE Booking SET Status = 'Cancelled' WHERE BookingID = @id AND MemberID = @m", con);
        cmd.Parameters.AddWithValue("@id", bookingId);
        cmd.Parameters.AddWithValue("@m", memberId);
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    public bool SetStatus(int bookingId, string status)
    {
        if (status != "Pending" && status != "Confirmed" && status != "Cancelled") return false;
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(
            "UPDATE Booking SET Status = @s WHERE BookingID = @id", con);
        cmd.Parameters.AddWithValue("@s", status);
        cmd.Parameters.AddWithValue("@id", bookingId);
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    public HomeStats GetStats()
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(@"
SELECT (SELECT COUNT(*) FROM Member)   AS Members,
       (SELECT COUNT(*) FROM Facility) AS Facilities,
       (SELECT COUNT(*) FROM Booking)  AS Bookings", con);
        con.Open();
        using var r = cmd.ExecuteReader();
        r.Read();
        return new HomeStats
        {
            Members = (int)r["Members"],
            Facilities = (int)r["Facilities"],
            Bookings = (int)r["Bookings"]
        };
    }

    private const string BaseSelect = @"
SELECT b.BookingID, b.MemberID, b.FacilityID, b.BookingDate, b.StartTime, b.EndTime,
       b.TotalPrice, b.Status, b.CreatedOn,
       m.FullName AS MemberName, m.Email AS MemberEmail,
       f.FacilityName, f.Location, f.HourlyRate, ft.TypeName
FROM Booking b
JOIN Member m ON m.MemberID = b.MemberID
JOIN Facility f ON f.FacilityID = b.FacilityID
JOIN FacilityType ft ON ft.FacilityTypeID = f.FacilityTypeID";

    private static Booking Map(SqlDataReader r) => new()
    {
        BookingID = (int)r["BookingID"],
        MemberID = (int)r["MemberID"],
        FacilityID = (int)r["FacilityID"],
        BookingDate = (DateTime)r["BookingDate"],
        StartTime = (TimeSpan)r["StartTime"],
        EndTime = (TimeSpan)r["EndTime"],
        TotalPrice = r["TotalPrice"] == DBNull.Value ? (decimal?)null : (decimal)r["TotalPrice"],
        Status = (string)r["Status"],
        CreatedOn = (DateTime)r["CreatedOn"],
        MemberName = (string)r["MemberName"],
        MemberEmail = (string)r["MemberEmail"],
        FacilityName = (string)r["FacilityName"],
        Location = (string)r["Location"],
        HourlyRate = (decimal)r["HourlyRate"],
        TypeName = (string)r["TypeName"]
    };
}