using System.Security.Cryptography;
using System.Text;

namespace TMIS.Utility
{
    public class SecurityBox
    {
        private const string keyString = "xP4sB9kQp7F6vZj3";
        private const string ivString = "L8qN7hR2VzW8ePf5";

        public static string EncryptString(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(keyString);
                aesAlg.IV = Encoding.UTF8.GetBytes(ivString);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new())
                using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                    swEncrypt.Close();
                    string base64 = Convert.ToBase64String(msEncrypt.ToArray());
                    // Make Base64 URL-safe
                    return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
                }
            }
        }

        public static string DecryptString(string cipherText)
        {
            // Reverse URL-safe Base64
            string base64 = cipherText.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(keyString);
                aesAlg.IV = Encoding.UTF8.GetBytes(ivString);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new(Convert.FromBase64String(base64)))
                using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}

