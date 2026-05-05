namespace MageritHealthAPI.Services.Interfaces
{
    public interface IAzureBlobService
    {
        Task<string> UploadImageAsync(IFormFile file, string nombreArchivo);
    }
}
