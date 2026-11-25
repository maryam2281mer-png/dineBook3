using System;
using System.Security.Cryptography;

namespace DineBook.Helpers
{
    /// <summary>
    /// Password hashing shared by Register, Login, AdminLogin and ManageUsers.
    /// Format: Base64(salt):Base64(hash)  -  PBKDF2 (HMAC-SHA1, the .NET Framework
    /// default of Rfc2898DeriveBytes), 16-byte salt, 32-byte hash, 100,000 iterations.
    /// This is the same format used by the seeded admin account in Database.sql.
    /// </summary>
    public static class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        public static string Hash(string password)
        {
            byte[] salt = new byte[SaltSize];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (Rfc2898DeriveBytes pbkdf2 =
                new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);
                return Convert.ToBase64String(salt) + ":" +
                       Convert.ToBase64String(hash);
            }
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(stored))
                return false;

            try
            {
                string[] parts = stored.Split(':');
                if (parts.Length != 2)
                    return false;

                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] savedHash = Convert.FromBase64String(parts[1]);

                byte[] enteredHash;
                using (Rfc2898DeriveBytes pbkdf2 =
                    new Rfc2898DeriveBytes(password, salt, Iterations))
                {
                    enteredHash = pbkdf2.GetBytes(savedHash.Length);
                }

                // Constant-time comparison
                int difference = savedHash.Length ^ enteredHash.Length;
                for (int i = 0; i < savedHash.Length && i < enteredHash.Length; i++)
                {
                    difference |= savedHash[i] ^ enteredHash[i];
                }

                return difference == 0;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
