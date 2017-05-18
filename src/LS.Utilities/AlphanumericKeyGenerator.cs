using System.Security.Cryptography;
using System.Text;

namespace LS.Utilities
{
    public class AlphanumericKeyGenerator
    {
        private static readonly string _charsForAlphanumericKeys = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";

        public static string GenerateAlphanumericKeyOfLength(int length)
        {
            var chars = _charsForAlphanumericKeys.ToCharArray();
            var data = new byte[1];

            using (var crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[length];
                crypto.GetNonZeroBytes(data);
            }
            var result = new StringBuilder(length);
            foreach (var b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }

            return result.ToString();
        }
    }
}