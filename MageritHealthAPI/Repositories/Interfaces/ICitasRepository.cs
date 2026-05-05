using MageritHealthAPI.Models;

namespace MageritHealthAPI.Repositories.Interfaces
{
    public interface ICitasRepository
    {
        // --- Consultas Generales ---
        Task<List<Cita>> GetCitasAsync();
        Task<Cita> FindCitaByIdAsync(int idCita);

        // --- Consultas específicas para Doctores ---
        Task<List<Cita>> GetHistorialCitasByIdDoctorAsync(int idDoctor);
        Task<List<Cita>> GetProximasCitasByIdDoctorAsync(int idDoctor, DateTime? fecha);
        Task<List<string>> GetHorasDisponiblesDoctorAsync(int idDoctor, DateTime fechaElegida);

        // --- Consultas específicas para Pacientes ---
        Task<List<Cita>> GetHistorialCitasByIdPacienteAsync(int idPaciente);
        Task<List<Cita>> GetProximasCitasByIdPacienteAsync(int idPaciente, DateTime? fecha);

        // --- Operaciones de Escritura y Estado ---
        Task<bool> CreateCitaAsync(Cita cita);
        Task<bool> UpdateCitaAsync(Cita updatedCita);
        Task<bool> UpdateEstadoCitaAsync(int idCita, string estado, bool activa);
    }
}
