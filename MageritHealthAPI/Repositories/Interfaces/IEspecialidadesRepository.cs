using MageritHealthAPI.Models;

namespace MageritHealthAPI.Repositories.Interfaces
{
    public interface IEspecialidadesRepository
    {
        public Task<List<Especialidad>> GetEspecialidadesAsync();

        public Task<Especialidad> GetEspecialidadByIdAsync(int id);

        Task<bool> CreateEspecialidadAsync(Especialidad especialidad);

        Task<bool> UpdateEspecialidadAsync(Especialidad especialidad);

        Task<bool> DeleteEspecialidadAsync(int id);
    }
}
