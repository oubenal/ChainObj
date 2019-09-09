using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace ChainObj
{
    internal static class Utilities
    {
        public static string ToFixedHex(this int val)
        {
            return val.ToString("X4");
        }
        public static string GetSha1(this string input)
        {
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                return string.Concat(hash.Select(_ => _.ToString("x2")));
            }
        }
        public static string Serialize(this object input) {
            return JsonConvert.SerializeObject(input);
        }
        public static T Deserialize<T>(this string output)
        {
            return JsonConvert.DeserializeObject<T>(output);
        }
        public static string GetSha1(this object input)
        {
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input.Serialize()));
                return string.Concat(hash.Select(_ => _.ToString("x2")));
            }
        }
        public static string RandomGuid => System.Guid.NewGuid().ToString("n").Substring(0, 8);
        public static string TempDirPath => $@"C:\temp\{RandomGuid}";
    }
}
