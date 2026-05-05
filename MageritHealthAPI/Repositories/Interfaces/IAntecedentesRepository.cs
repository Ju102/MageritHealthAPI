using MageritHealthAPI.Models;

namespace MageritHealthAPI.Repositories.Interfaces
{
    public interface IAntecedentesRepository
    {
        Task<List<AntecedenteMedico>> GetAntecedentesByIdPacienteAsync(int idPaciente);

        Task<AntecedenteMedico> GetAntecedenteByIdAsync(int idAntecedente);

        Task<bool> CreateAntecedenteAsync(AntecedenteMedico antecedente);

        Task<bool> UpdateAntecedenteAsync(AntecedenteMedico antecedente);

        Task<bool> UpdateEstadoAntecedenteAsync(int idAntecedente, bool activo);

        Task<bool> DeleteAntecedenteAsync(int idAntecedente);
    }
}