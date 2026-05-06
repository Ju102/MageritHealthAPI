using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MageritHealthAPI.Services.Interfaces;

namespace MageritHealthAPI.Services
{
    public class AzureBlobService : IAzureBlobService
    {
        private readonly string connectionString;
        private readonly string containerName;

        public AzureBlobService(IConfiguration configuration)
        {
            // Se recupera el secreto inyectado desde el Key Vault
            this.connectionString = configuration["AzureStorageConnectionString"];

            // Se obtiene el nombre del contenedor desde la configuración local (appsettings.json)
            this.containerName = configuration["AzureStorageConfig:ContainerImagenes"];
        }

        public async Task<string> UploadImageAsync(IFormFile file, string nombreArchivo)
        {
            // Se valida la existencia de la cadena de conexión antes de proceder
            if (string.IsNullOrEmpty(this.connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión de Azure Storage no se ha encontrado en el Key Vault.");
            }

            // Se crea el cliente principal para interactuar con el servicio de Azure Storage
            BlobServiceClient blobServiceClient = new BlobServiceClient(this.connectionString);

            // Se obtiene una referencia al contenedor de blobs especificado
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(this.containerName);

            // Se genera el cliente para el blob específico usando el nombre proporcionado
            BlobClient blobClient = containerClient.GetBlobClient(nombreArchivo);

            // Se abre el flujo de datos del archivo y se inicia la carga
            using (var stream = file.OpenReadStream())
            {
                // Se realiza la subida de forma asíncrona y se define el tipo de contenido (MIME type)
                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
                });
            }

            // Se devuelve la dirección URL absoluta del recurso almacenado
            return blobClient.Uri.ToString();
        }
    }
}