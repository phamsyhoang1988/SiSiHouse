using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;

namespace SiSiHouse.Common
{

    public class SafePassword
    {
        private static int STRETCH_COUNT = 1000;

        public static string GetStretchedPassword(string password, string userId)
        {
            string salt = GetSha256(userId);
            string hash = "";

            for (int i = 0; i < STRETCH_COUNT; i++)
            {
                hash = GetSha256(hash + salt + password);
            }

            return hash;
        }

        public static String GetSha256(string target)
        {
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] byteValue = Encoding.UTF8.GetBytes(target);
            byte[] hash = mySHA256.ComputeHash(byteValue);

            StringBuilder buf = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                buf.AppendFormat("{0:x2}", hash[i]);
            }

            return buf.ToString();
        }
    }
}
