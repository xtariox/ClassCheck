using System.Security.Cryptography;
using System.Text;

namespace ClassCheck.Services
{
    public class SecurityService
    {
        // Method to hash a password using SHA256
        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Method to verify entered password against stored hash
        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            string hashOfInput = HashPassword(enteredPassword);
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, storedHash) == 0;
        }
    }
}