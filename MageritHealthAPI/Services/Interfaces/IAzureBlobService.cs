namespace MageritHealthAPI.Services.Interfaces
{
    public interface IAzureBlobService
    {
        // Para imágenes desde un formulario (IFormFile)
        Task<string> UploadFileAsync(IFormFile file, string nombreArchivo, string containerName);

        // Para PDFs generados en memoria (byte[])
        Task<string> UploadFileAsync(byte[] fileData, string nombreArchivo, string containerName, string contentType);

        // Para descargas
        Task<Stream> GetBlobStreamAsync(string fileName, string containerName);
    }
}
