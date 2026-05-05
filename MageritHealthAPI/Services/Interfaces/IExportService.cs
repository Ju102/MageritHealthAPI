namespace MageritHealthAPI.Services.Interfaces
{
    public interface IExportService
    {
        Task<byte[]> GenerarInformeAnaliticaPdfAsync(int idAnalitica);

        Task<byte[]> GenerarRecetasPorCitaPdfAsync(int idCita);

        Task<byte[]> GenerarInformeCitaPdfAsync(int idCita);
    }
}