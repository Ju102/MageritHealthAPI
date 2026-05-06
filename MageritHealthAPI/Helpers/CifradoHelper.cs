using System.Security.Cryptography;
using System.Text;

namespace MageritHealthAPI.Helpers
{
    public static class CifradoHelper
    {
        private static string KeyCifrado;

        public static void Initialize(IConfiguration config)
        {
            // Se extrae la clave de cifrado desde la sección correspondiente de la configuración
            KeyCifrado = config.GetValue<string>("Cypher:Key");
        }

        public static string CifrarString(string data)
        {
            // Se convierte la clave de texto a un array de bytes (UTF8)
            byte[] keyData = Encoding.UTF8.GetBytes(KeyCifrado);

            // Se ejecuta el proceso de cifrado mediante la función interna
            string cifrado = EncryptString(keyData, data);

            return cifrado;
        }

        public static string DescifrarString(string data)
        {
            // Se obtiene la representación en bytes de la clave configurada
            byte[] keyData = Encoding.UTF8.GetBytes(KeyCifrado);

            // Se invoca el método encargado de revertir el cifrado del texto
            string result = DecryptString(keyData, data);

            return result;
        }

        private static string EncryptString(byte[] key, string plainText)
        {
            // Se define un vector de inicialización (IV) de 16 bytes con valores por defecto
            byte[] iv = new byte[16];
            byte[] array;

            // Se inicializa el motor del algoritmo AES
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                // Se genera el transformador encargado de la cifración
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            // Se escribe el texto plano en el flujo criptográfico
                            streamWriter.Write(plainText);
                        }

                        // Se capturan los bytes resultantes del flujo de memoria
                        array = memoryStream.ToArray();
                    }
                }
            }

            // Se retorna el resultado codificado en formato Base64 para su almacenamiento o transporte
            return Convert.ToBase64String(array);
        }

        private static string DecryptString(byte[] key, string cipherText)
        {
            // Se establece el IV y se decodifica la cadena de entrada desde Base64 a bytes
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            // Se configura la instancia de AES para la operación de descifrado
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                // Se crea el objeto transformador para la lectura de datos cifrados
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            // Se lee el flujo completo hasta obtener el texto original
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}