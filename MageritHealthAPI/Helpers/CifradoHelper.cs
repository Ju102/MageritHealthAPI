using System.Security.Cryptography;
using System.Text;

namespace MageritHealthAPI.Helpers
{
    public static class CifradoHelper
    {
        private static string KeyCifrado;

        public static void Initialize(IConfiguration config)
        {
            KeyCifrado = config.GetValue<string>("Cypher:Key");
        }

        public static string CifrarString(string data)
        {
            // Se convierte a byte[] la clave
            byte[] keyData = Encoding.UTF8.GetBytes(KeyCifrado);
            string cifrado = EncryptString(keyData, data);

            return cifrado;
        }

        public static string DescifrarString(string data)
        {
            // Se convierte a byte[] la clave
            byte[] keyData = Encoding.UTF8.GetBytes(KeyCifrado);
            string result = DecryptString(keyData, data);

            return result;
        }

        private static string EncryptString(byte[] key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        private static string DecryptString(byte[] key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
