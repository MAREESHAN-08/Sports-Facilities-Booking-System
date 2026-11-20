using System.Security.Cryptography;
using System.Text;

namespace SportsBookingApp.Data;

public static class PasswordHasher
{
    public static string Hash(string password)
    {
        using var sha = SHA256.Create();
        byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password ?? ""));
        return Convert.ToHexString(bytes);   // 64-char uppercase hex
    }
}