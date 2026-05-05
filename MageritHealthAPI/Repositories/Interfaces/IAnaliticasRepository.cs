using MageritHealthAPI.Models;

namespace MageritHealthAPI.Repositories.Interfaces
{
    public interface IAnaliticasRepository
    {
        Task<Analitica> FindAnaliticaByIdAsync(int idAnalitica);

        Task<List<Analitica>> GetAnaliticasByIdPacienteAsync(int idPaciente);

        Task<List<Analitica>> GetAnaliticasByIdDoctorAsync(int idDoctor);

        Task<bool> CreateAnaliticaAsync(Analitica analitica);

        Task<bool> UpdateFechaAnaliticaAsync(int idAnalitica, DateTime nuevaFecha);

        Task<bool> UpdateEstadoAnaliticaAsync(int idAnalitica, string estado);

        Task<bool> FinalizarAnaliticaAsync(int idAnalitica, List<Medicion> mediciones);

        Task<List<Medicion>> GetListaMedicionesByIdAnaliticaAsync(int idAnalitica);

    }
}
