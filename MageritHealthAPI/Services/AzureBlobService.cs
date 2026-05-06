using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MageritHealthAPI.Services.Interfaces;

namespace MageritHealthAPI.Services
{
    public class AzureBlobService : IAzureBlobService
    {
        private readonly string connectionString;
        private readonly BlobServiceClient blobServiceClient;

        public AzureBlobService(IConfiguration configuration)
        {
            // Se recupera la cadena de conexión desde los Secrets de configuración
            this.connectionString = configuration["AzureStorageConnectionString"];

            // Se comprueba la validez de la cadena de conexión antes de la inicialización
            if (string.IsNullOrEmpty(this.connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión de Azure Storage no se encuentra.");
            }

            // Se instancia el cliente de Blob Service una sola vez para optimizar recursos
            this.blobServiceClient = new BlobServiceClient(this.connectionString);
        }

        // ================================================================
        // SUBIDA DESDE FORMULARIO (Imágenes de perfil)
        // ================================================================
        public async Task<string> UploadFileAsync(IFormFile file, string nombreArchivo, string containerName)
        {
            // Se obtiene la referencia al contenedor y al blob específico
            BlobContainerClient containerClient = this.blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(nombreArchivo);

            // Se abre el flujo de lectura del archivo recibido
            using (var stream = file.OpenReadStream())
            {
                // Se realiza la carga asíncrona y se asignan las cabeceras HTTP de tipo de contenido
                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
                });
            }

            // Se devuelve la URI absoluta del recurso cargado
            return blobClient.Uri.ToString();
        }

        // ================================================================
        // SUBIDA DESDE ARRAY DE BYTES (Para PDFs generados)
        // ================================================================
        public async Task<string> UploadFileAsync(byte[] fileData, string nombreArchivo, string containerName, string contentType)
        {
            // Se localiza el contenedor y se prepara el cliente para el nuevo blob
            BlobContainerClient containerClient = this.blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(nombreArchivo);

            // Se crea un flujo de memoria a partir del arreglo de bytes
            using (var stream = new MemoryStream(fileData))
            {
                // Se transfiere el flujo a Azure Storage con el tipo de contenido especificado
                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
                });
            }

            // Se retorna la dirección URL del archivo almacenado
            return blobClient.Uri.ToString();
        }

        // ================================================================
        // DESCARGA DE ARCHIVOS
        // ================================================================
        public async Task<Stream> GetBlobStreamAsync(string fileName, string containerName)
        {
            // Se establece la conexión con el contenedor y el blob solicitado
            BlobContainerClient containerClient = this.blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            // Se verifica la existencia del archivo en el almacenamiento antes de la lectura
            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            // Se abre y se devuelve el flujo de lectura directamente desde el servicio de Azure
            var response = await blobClient.OpenReadAsync();
            return response;
        }
    }
}