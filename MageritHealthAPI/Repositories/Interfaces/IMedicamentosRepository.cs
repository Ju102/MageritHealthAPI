using MageritHealthAPI.Models;

namespace MageritHealthAPI.Repositories.Interfaces
{
    public interface IMedicamentosRepository
    {
        Task<List<Medicamento>> GetMedicamentosAsync();
        Task<Medicamento> GetMedicamentoByIdAsync(int id);
        Task<List<Medicamento>> SearchMedicamentosAsync(string searchTerm);
        Task<bool> CreateMedicamentoAsync(Medicamento medicamento);
        Task<bool> UpdateMedicamentoAsync(Medicamento medicamento);
        Task<bool> UpdateEstadoMedicamentoAsync(int id, bool nuevoEstado);
        Task<bool> DeleteMedicamentoAsync(int id);

    }
}
