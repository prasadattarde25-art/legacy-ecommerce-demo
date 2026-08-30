using System;
using System.Security.Cryptography;

namespace Ecommerce.Services.Security
{
    /// <summary>
    /// PBKDF2 password hashing (Rfc2898DeriveBytes). Salt and hash are
    /// stored as base64 on the Customer row — legacy-friendly, no extra deps.
    /// </summary>
    public static class PasswordHasher
    {
        private const int Iterations = 10000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        public static string GenerateSalt()
        {
            var salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        public static string Hash(string password, string saltBase64)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (saltBase64 == null) throw new ArgumentNullException("saltBase64");

            var salt = Convert.FromBase64String(saltBase64);
            using (var derive = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                return Convert.ToBase64String(derive.GetBytes(HashSize));
            }
        }

        public static bool Verify(string password, string saltBase64, string expectedHashBase64)
        {
            if (string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(saltBase64) ||
                string.IsNullOrEmpty(expectedHashBase64))
            {
                return false;
            }

            string actual;
            try
            {
                actual = Hash(password, saltBase64);
            }
            catch (Exception)
            {
                return false;
            }

            return FixedTimeEquals(actual, expectedHashBase64);
        }

        private static bool FixedTimeEquals(string a, string b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;

            byte[] left, right;
            try
            {
                left = Convert.FromBase64String(a);
                right = Convert.FromBase64String(b);
            }
            catch (FormatException)
            {
                return false;
            }

            var diff = 0;
            for (var i = 0; i < left.Length; i++)
            {
                diff |= left[i] ^ right[i];
            }

            return diff == 0;
        }
    }
}