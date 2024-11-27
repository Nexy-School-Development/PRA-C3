using System;
using System.Security.Cryptography;
using System.Text;

namespace backend.Models;

public class User
{
    public int Id { get; set; }
    public string? Email { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
    public string? Goals { get; set; } = "0";
    public bool? IsAdmin { get; set; }
    public string? Token { get; set; } = string.Empty;

    public User()
    {
        GenerateToken();
    }

    public void GenerateToken()
    {
        string toHash = Email + Password;
        Token = ComputeSha256Hash(toHash);
    }

    public bool ValidatePassword(string password)
    {
        string hashedPw = ComputeSha256Hash(password);
        return Password == hashedPw;
    }

    public static string ComputeSha256Hash(string rawData)
    {
        // Create a SHA256
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
