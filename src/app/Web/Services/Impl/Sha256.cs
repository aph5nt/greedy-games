using System.Security.Cryptography;
using System.Text;

namespace Web.Services.Impl
{
    public class SHA256
    {
        public static string Encode(string password)
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));

            foreach (var theByte in crypto)
                hash.Append(theByte.ToString("x2"));

            return hash.ToString();
        }

        public static bool Equals(string secret, string hash)
        {
            var hashedSecret = Encode(secret);
            return hashedSecret.Equals(hash);
        }
    }
}