using MageritHealthAPI.Models;

namespace MageritHealthAPI.Repositories.Interfaces
{
    public interface IPrescripcionesRepository
    {
        public Task<List<Prescripcion>> GetPrescripcionesByPacienteIdAsync(int pacienteId);

        public Task<Prescripcion> GetPrescripcionByIdAsync(int id);

        Task<bool> CreatePrescripcionAsync(Prescripcion prescripcion);
        
        Task<bool> UpdateEstadoPrescripcionAsync(int id, bool nuevoEstado);
        
        Task<bool> UpdatePrescripcionAsync(Prescripcion prescripcion);

        Task<bool> DeletePrescripcionAsync(int id);
    }
}
