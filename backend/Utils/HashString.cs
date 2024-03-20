using System.Security.Cryptography;
using System.Text;

namespace backend.Utils
{
    public class HashString
    {
        public static string HashWithSHA256(string value)
        {
            using var hash = SHA256.Create();
            var byteArray = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Convert.ToHexString(byteArray);
        }
    }
}
