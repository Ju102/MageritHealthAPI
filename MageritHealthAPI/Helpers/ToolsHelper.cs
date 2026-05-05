using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace MageritHealthAPI.Helpers
{
    public class ToolsHelper
    {
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[36];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static bool CompareArrays(byte[] a, byte[] b)
        {
            bool iguales = true;

            if (a.Length != b.Length)
            {
                iguales = false;
            }
            else
            {
                for (int i = 0; i < a.Length; i++)
                {
                    if (!a[i].Equals(b[i]))
                    {
                        iguales = false;
                        break;
                    }
                }
            }

            return iguales;
        }

        public static string GenerateRandomPassword()
        {
            int longitud = 10;

            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@#*-_";
            char[] passwordGenerada = new char[longitud];

            for (int i = 0; i < longitud; i++)
            {
                int randomIndex = RandomNumberGenerator.GetInt32(validChars.Length);
                passwordGenerada[i] = validChars[randomIndex];
            }

            return new string(passwordGenerada);
        }
    }
}
