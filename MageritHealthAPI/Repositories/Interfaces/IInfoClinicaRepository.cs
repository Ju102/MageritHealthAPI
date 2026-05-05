using MageritHealthAPI.Models;

namespace MageritHealthAPI.Repositories.Interfaces
{
    public interface IInfoClinicaRepository
    {
        Task<InfoClinicaPaciente> GetInfoClinicaByIdPacienteAsync(int idPaciente);

        Task<bool> CreateInfoClinicaAsync(InfoClinicaPaciente infoClinica);

        Task<bool> UpdateInfoClinicaAsync(InfoClinicaPaciente infoClinica);
    }
}