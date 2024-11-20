using System;
using System.Security.Cryptography;
using System.Text;

namespace backend.models;

public class User
{
    public int id { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public string IsAdmin { get; set; }
    public string Token { get; set; }

    public User()
    {
        string toHash = email + password;
        Token = ComputeSha256Hash(toHash);
    }

    static string ComputeSha256Hash(string rawData)
    {
        // Create a SHA256
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // ComputeHash - returns byte array
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            // Convert byte array to a string
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

};
