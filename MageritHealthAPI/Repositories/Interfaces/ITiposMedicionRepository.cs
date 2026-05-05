using MageritHealthAPI.Models;

namespace MageritHealthAPI.Repositories.Interfaces
{
    public interface ITiposMedicionRepository
    {
        Task<List<TipoMedicion>> GetTiposMedicionAsync();
        Task<TipoMedicion> GetTipoMedicionByIdAsync(int id);
        Task<bool> CreateTipoMedicionAsync(TipoMedicion tipoMedicion);
        Task<bool> UpdateTipoMedicionAsync(TipoMedicion tipoMedicion);
        Task<bool> UpdateEstadoTipoMedicionAsync(int id, bool activo);
        Task<bool> DeleteTipoMedicionAsync(int id);
    }
}
