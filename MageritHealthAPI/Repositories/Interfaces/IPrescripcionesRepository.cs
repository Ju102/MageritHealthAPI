using MageritHealthAPI.Models;

namespace MageritHealthAPI.Repositories.Interfaces
{
    public interface IPrescripcionesRepository
    {
        Task<List<Prescripcion>> GetPrescripcionesByIdDoctorAsync(int idDoctor);

        Task<List<Prescripcion>> GetPrescripcionesByIdPacienteAsync(int idPaciente);

        Task<List<Prescripcion>> GetPrescripcionesByIdCitaAsync(int idCita);

        Task<Prescripcion> GetPrescripcionByIdAsync(int id);

        Task<bool> CreatePrescripcionAsync(Prescripcion prescripcion);
        
        Task<bool> UpdateEstadoPrescripcionAsync(int id, bool nuevoEstado);
        
        Task<bool> UpdatePrescripcionAsync(Prescripcion prescripcion);

        Task<bool> DeletePrescripcionAsync(int id);
    }
}
