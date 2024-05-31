using System;
using System.Security.Cryptography;
using System.Text;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        // Generate a salt
        byte[] salt;
        new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

        // Create the Rfc2898DeriveBytes and get the hash value
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
        byte[] hash = pbkdf2.GetBytes(20);

        // Combine the salt and password bytes for later use
        byte[] hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        // Convert the resulting hash to a string
        return Convert.ToBase64String(hashBytes);
    }

    public static bool VerifyPassword(string enteredPassword, string storedHash)
    {
        // Extract the bytes
        byte[] hashBytes = Convert.FromBase64String(storedHash);

        // Get the salt
        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        // Compute the hash on the password the user entered
        var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000);
        byte[] hash = pbkdf2.GetBytes(20);

        // Compare the results
        for (int i = 0; i < 20; i++)
        {
            if (hashBytes[i + 16] != hash[i])
                return false;
        }

        return true;
    }
}
