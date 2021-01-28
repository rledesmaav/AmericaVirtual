using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace ClimaAV.Helpers
{
    public class Encryption
    {
        public static string Encrypt(string text)
        {
            SHA512 hash = new System.Security.Cryptography.SHA512Managed();
            byte[] textBytes = System.Text.Encoding.Default.GetBytes(text);
            byte[] hashBytes = hash.ComputeHash(textBytes);
            string result = string.Empty;

            for (int i = 0; i < hashBytes.Length; i++)
            {
                result += hashBytes[i].ToString("X");
            }

            return result;
        }
    }
}