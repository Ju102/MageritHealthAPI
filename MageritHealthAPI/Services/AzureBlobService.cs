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
            // Leemos el secreto inyectado desde el Key Vault
            this.connectionString = configuration["AzureStorageConnectionString"];

            // Leemos el nombre del contenedor desde appsettings.json
            this.containerName = configuration["AzureStorageConfig:ContainerImagenes"];
        }

        public async Task<string> UploadImageAsync(IFormFile file, string nombreArchivo)
        {
            if (string.IsNullOrEmpty(this.connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión de Azure Storage no se ha encontrado en el Key Vault.");
            }

            BlobServiceClient blobServiceClient = new BlobServiceClient(this.connectionString);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(this.containerName);

            BlobClient blobClient = containerClient.GetBlobClient(nombreArchivo);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
                });
            }

            return blobClient.Uri.ToString();
        }
    }
}