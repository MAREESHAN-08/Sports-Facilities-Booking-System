using Microsoft.Data.SqlClient;
using SportsBookingApp.Models;

namespace SportsBookingApp.Data;

public class InquiryRepository
{
    private readonly string _cs;
    public InquiryRepository(IConfiguration config)
        => _cs = config.GetConnectionString("DefaultConnection");

    public void Add(Inquiry inq)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(@"
INSERT INTO Inquiry (GuestName, GuestEmail, Subject, Message)
VALUES (@n, @e, @s, @msg)", con);
        cmd.Parameters.AddWithValue("@n", inq.GuestName);
        cmd.Parameters.AddWithValue("@e", inq.GuestEmail);
        cmd.Parameters.AddWithValue("@s", (object)inq.Subject ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@msg", inq.Message);
        con.Open();
        cmd.ExecuteNonQuery();
    }
}