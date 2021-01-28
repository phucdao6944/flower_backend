using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace flower.Helpers
{
    public class Security
    {
        public static string HexEncode(byte[] aby)
        {
            string hex = "0123456789abcdef";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < aby.Length; i++)
            {
                sb.Append(hex[(aby[i] & 0xf0) >> 4]);
                sb.Append(hex[aby[i] & 0x0f]);
            }
            return sb.ToString();
        }
        public static string HashPassword(string sPASSWORD)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] aby = utf8.GetBytes(sPASSWORD);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] binMD5 = md5.ComputeHash(aby);
                return HexEncode(binMD5);
            }
        }
    }
}
