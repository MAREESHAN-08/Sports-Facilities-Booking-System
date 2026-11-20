using Microsoft.Data.SqlClient;
using SportsBookingApp.Models;
using SportsBookingApp.ViewModels;

namespace SportsBookingApp.Data;

public class MemberRepository
{
    private readonly string _cs;
    public MemberRepository(IConfiguration config)
        => _cs = config.GetConnectionString("DefaultConnection");

    public bool EmailExists(string email)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand("SELECT COUNT(1) FROM Member WHERE Email = @e", con);
        cmd.Parameters.AddWithValue("@e", email);
        con.Open();
        return (int)cmd.ExecuteScalar() > 0;
    }

    public int Create(RegisterViewModel m, string passwordHash)
    {
        using var con = new SqlConnection(_cs);
        con.Open();
        using var tx = con.BeginTransaction();
        try
        {
            int memberId;
            using (var cmd = new SqlCommand(
                @"INSERT INTO Member (FullName, Email, Password, Phone, Address)
                  OUTPUT INSERTED.MemberID
                  VALUES (@n, @e, @p, @ph, @a)", con, tx))
            {
                cmd.Parameters.AddWithValue("@n", m.FullName);
                cmd.Parameters.AddWithValue("@e", m.Email);
                cmd.Parameters.AddWithValue("@p", passwordHash);
                cmd.Parameters.AddWithValue("@ph", (object)m.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@a", (object)m.Address ?? DBNull.Value);
                memberId = (int)cmd.ExecuteScalar();
            }

            if (m.SelectedSportIds != null && m.SelectedSportIds.Count > 0)
            {
                foreach (int sid in m.SelectedSportIds.Distinct())
                {
                    using var c2 = new SqlCommand(
                        "INSERT INTO MemberSport (MemberID, SportID) VALUES (@m, @s)", con, tx);
                    c2.Parameters.AddWithValue("@m", memberId);
                    c2.Parameters.AddWithValue("@s", sid);
                    c2.ExecuteNonQuery();
                }
            }

            tx.Commit();
            return memberId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public Member Authenticate(string email, string password)
    {
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(
            @"SELECT MemberID, FullName, Email, Phone, Address, RegistrationDate
              FROM Member WHERE Email = @e AND Password = @p", con);
        cmd.Parameters.AddWithValue("@e", email);
        cmd.Parameters.AddWithValue("@p", PasswordHasher.Hash(password));
        con.Open();
        using var r = cmd.ExecuteReader();
        return r.Read() ? Map(r) : null;
    }

    public Member GetOrCreateGuestMember()
    {
        const string guestEmail = "guest@arenapulse.com";
        using var con = new SqlConnection(_cs);
        con.Open();

        // 1. Try to find existing guest account
        using (var cmd = new SqlCommand(
            @"SELECT MemberID, FullName, Email, Phone, Address, RegistrationDate
              FROM Member WHERE Email = @e", con))
        {
            cmd.Parameters.AddWithValue("@e", guestEmail);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                return Map(r);
            }
        }

        // 2. If not found, create one
        string passwordHash = PasswordHasher.Hash("Guest@123");
        using (var cmd = new SqlCommand(
            @"INSERT INTO Member (FullName, Email, Password, Phone, Address)
              OUTPUT INSERTED.MemberID, INSERTED.FullName, INSERTED.Email, INSERTED.Phone, INSERTED.Address, INSERTED.RegistrationDate
              VALUES (@n, @e, @p, @ph, @a)", con))
        {
            cmd.Parameters.AddWithValue("@n", "Guest Player");
            cmd.Parameters.AddWithValue("@e", guestEmail);
            cmd.Parameters.AddWithValue("@p", passwordHash);
            cmd.Parameters.AddWithValue("@ph", "07000 000000");
            cmd.Parameters.AddWithValue("@a", "Community Guest Zone");

            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                return Map(r);
            }
        }

        throw new InvalidOperationException("Failed to initialize guest member account.");
    }

    public List<Sport> GetAllSports()
    {
        var list = new List<Sport>();
        using var con = new SqlConnection(_cs);
        using var cmd = new SqlCommand(
            "SELECT SportID, SportName, Description FROM Sport ORDER BY SportName", con);
        con.Open();
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Sport
            {
                SportID = (int)r["SportID"],
                SportName = (string)r["SportName"],
                Description = r["Description"] == DBNull.Value ? null : (string)r["Description"]
            });
        }
        return list;
    }

    private static Member Map(SqlDataReader r) => new()
    {
        MemberID = (int)r["MemberID"],
        FullName = (string)r["FullName"],
        Email = (string)r["Email"],
        Phone = r["Phone"] == DBNull.Value ? null : (string)r["Phone"],
        Address = r["Address"] == DBNull.Value ? null : (string)r["Address"],
        RegistrationDate = (DateTime)r["RegistrationDate"]
    };
}