using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using SportsBookingApp.Models;

namespace SportsBookingApp.Data;

public class FacilityRepository
{
    private readonly string _cs;
    public FacilityRepository(IConfiguration config)
        => _cs = config.GetConnectionString("DefaultConnection");

    public List<FacilityType> GetTypes()
    {
        var list = new List<FacilityType>();
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(
            "SELECT FacilityTypeID, TypeName, Description FROM FacilityType ORDER BY TypeName", con);
        con.Open();
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new FacilityType
            {
                FacilityTypeID = (int)r["FacilityTypeID"],
                TypeName = (string)r["TypeName"],
                Description = r["Description"] == DBNull.Value ? null : (string)r["Description"]
            });
        }
        return list;
    }

    public List<Facility> Search(int? typeId, string location, DateTime? date, TimeSpan? start, TimeSpan? end)
    {
        var list = new List<Facility>();

        var sql = new StringBuilder(@"
SELECT f.FacilityID, f.FacilityTypeID, f.FacilityName, f.Location, f.Capacity,
       f.HourlyRate, f.ContactPhone, f.Description, f.IsAvailable, ft.TypeName,
       (SELECT AVG(CAST(Rating AS FLOAT)) FROM Review r WHERE r.FacilityID = f.FacilityID) AS AvgRating,
       (SELECT COUNT(*) FROM Review r WHERE r.FacilityID = f.FacilityID) AS ReviewCount
FROM Facility f
JOIN FacilityType ft ON ft.FacilityTypeID = f.FacilityTypeID
WHERE f.IsAvailable = 1");

        if (typeId.HasValue)
            sql.Append(" AND f.FacilityTypeID = @TypeId");
        if (!string.IsNullOrWhiteSpace(location))
            sql.Append(" AND f.Location LIKE @Loc");
        if (date.HasValue && start.HasValue && end.HasValue)
            sql.Append(@" AND NOT EXISTS
                (SELECT 1 FROM Booking b
                 WHERE b.FacilityID = f.FacilityID
                   AND b.Status <> 'Cancelled'
                   AND b.BookingDate = @Date
                   AND b.StartTime < @End AND b.EndTime > @Start)");

        sql.Append(" ORDER BY f.FacilityName");

        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(sql.ToString(), con);

        if (typeId.HasValue) cmd.Parameters.AddWithValue("@TypeId", typeId.Value);
        if (!string.IsNullOrWhiteSpace(location)) cmd.Parameters.AddWithValue("@Loc", "%" + location.Trim() + "%");
        if (date.HasValue && start.HasValue && end.HasValue)
        {
            cmd.Parameters.Add("@Date", SqlDbType.Date).Value = date.Value.Date;
            cmd.Parameters.Add("@Start", SqlDbType.Time).Value = start.Value;
            cmd.Parameters.Add("@End", SqlDbType.Time).Value = end.Value;
        }

        con.Open();
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(MapFacility(r));
        return list;
    }

    public List<Facility> GetFeatured(int n)
    {
        var list = new List<Facility>();
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(@"
SELECT TOP (@n) f.FacilityID, f.FacilityTypeID, f.FacilityName, f.Location, f.Capacity,
       f.HourlyRate, f.ContactPhone, f.Description, f.IsAvailable, ft.TypeName,
       (SELECT AVG(CAST(Rating AS FLOAT)) FROM Review r WHERE r.FacilityID = f.FacilityID) AS AvgRating,
       (SELECT COUNT(*) FROM Review r WHERE r.FacilityID = f.FacilityID) AS ReviewCount
FROM Facility f
JOIN FacilityType ft ON ft.FacilityTypeID = f.FacilityTypeID
WHERE f.IsAvailable = 1
ORDER BY CASE WHEN (SELECT AVG(CAST(Rating AS FLOAT)) FROM Review r WHERE r.FacilityID = f.FacilityID) IS NULL THEN 1 ELSE 0 END,
         (SELECT AVG(CAST(Rating AS FLOAT)) FROM Review r WHERE r.FacilityID = f.FacilityID) DESC,
         f.FacilityName", con);
        cmd.Parameters.AddWithValue("@n", n);
        con.Open();
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(MapFacility(r));
        return list;
    }

    public Facility GetById(int id)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(@"
SELECT f.FacilityID, f.FacilityTypeID, f.FacilityName, f.Location, f.Capacity,
       f.HourlyRate, f.ContactPhone, f.Description, f.IsAvailable, ft.TypeName,
       (SELECT AVG(CAST(Rating AS FLOAT)) FROM Review r WHERE r.FacilityID = f.FacilityID) AS AvgRating,
       (SELECT COUNT(*) FROM Review r WHERE r.FacilityID = f.FacilityID) AS ReviewCount
FROM Facility f
JOIN FacilityType ft ON ft.FacilityTypeID = f.FacilityTypeID
WHERE f.FacilityID = @id", con);
        cmd.Parameters.AddWithValue("@id", id);
        con.Open();
        using var r = cmd.ExecuteReader();
        return r.Read() ? MapFacility(r) : null;
    }

    private static Facility MapFacility(SqlDataReader r) => new()
    {
        FacilityID = (int)r["FacilityID"],
        FacilityTypeID = (int)r["FacilityTypeID"],
        FacilityName = (string)r["FacilityName"],
        TypeName = (string)r["TypeName"],
        Location = (string)r["Location"],
        Capacity = (int)r["Capacity"],
        HourlyRate = (decimal)r["HourlyRate"],
        ContactPhone = r["ContactPhone"] == DBNull.Value ? null : (string)r["ContactPhone"],
        Description = r["Description"] == DBNull.Value ? null : (string)r["Description"],
        IsAvailable = (bool)r["IsAvailable"],
        AvgRating = r["AvgRating"] == DBNull.Value ? (double?)null : Convert.ToDouble(r["AvgRating"]),
        ReviewCount = (int)r["ReviewCount"]
    };
}