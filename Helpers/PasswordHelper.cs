using System;
using System.Security.Cryptography;
using System.Text;

namespace QuizAppDotNetFrameWork.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);

                // Use this method instead
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash)) return false;

            string enteredHash = HashPassword(enteredPassword);
            return enteredHash.Equals(storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}