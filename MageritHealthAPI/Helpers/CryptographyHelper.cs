using System.Security.Cryptography;
using System.Text;

namespace MageritHealthAPI.Helpers
{
    public class CryptographyHelper
    {
        public static byte[] EncryptPassword(string password, string salt)
        {
            string content = password + salt.Trim();
            byte[] salida = Encoding.UTF8.GetBytes(content);

            using (SHA512 managed = SHA512.Create())
            {
                for (int i = 1; i <= 100000; i++)
                {
                    salida = managed.ComputeHash(salida);
                }
            }
            return salida;
        }
    }
}
