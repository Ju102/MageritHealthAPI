using MageritHealthAPI.Data;
using MageritHealthAPI.Models;
using MageritHealthAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MageritHealthAPI.Repositories
{
    public class MedicamentosRepository : IMedicamentosRepository
    {
        private readonly MageritHealthDbContext context;

        public MedicamentosRepository(MageritHealthDbContext context)
        {
            this.context = context;
        }

        public async Task<Medicamento> GetMedicamentoByIdAsync(int id)
        {
            return await this.context.Medicamentos.FirstOrDefaultAsync(m => m.IdMedicamento == id);
        }

        public async Task<List<Medicamento>> GetMedicamentosAsync()
        {
            return await this.context.Medicamentos.ToListAsync();
        }

        public async Task<List<Medicamento>> SearchMedicamentosAsync(string searchTerm)
        {
            return await this.context.Medicamentos
                .Where(m => m.NombreComercial.Contains(searchTerm) || m.Fabricante.Contains(searchTerm))
                .ToListAsync();
        }

        // --- MÉTODOS DE ESCRITURA ---

        public async Task<bool> CreateMedicamentoAsync(Medicamento medicamento)
        {
            int maxId = await this.context.Medicamentos.MaxAsync(m => (int?)m.IdMedicamento) ?? 0;
            medicamento.IdMedicamento = maxId + 1;
            medicamento.Activo = true;

            await this.context.Medicamentos.AddAsync(medicamento);
            return await this.context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateMedicamentoAsync(Medicamento medicamento)
        {
            Medicamento storedMedicamento = await this.context.Medicamentos.FirstOrDefaultAsync(m => m.IdMedicamento == medicamento.IdMedicamento);

            if (storedMedicamento != null)
            {
                storedMedicamento.NombreComercial = medicamento.NombreComercial;
                storedMedicamento.Fabricante = medicamento.Fabricante;
                storedMedicamento.PrincipioActivo = medicamento.PrincipioActivo;
                storedMedicamento.Concentracion = medicamento.Concentracion;
                storedMedicamento.Formato = medicamento.Formato;

                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> UpdateEstadoMedicamentoAsync(int id, bool nuevoEstado)
        {
            Medicamento medicamento = await this.context.Medicamentos.FirstOrDefaultAsync(m => m.IdMedicamento == id);

            if (medicamento != null)
            {
                medicamento.Activo = nuevoEstado;
                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> DeleteMedicamentoAsync(int id)
        {
            Medicamento medicamento = await this.context.Medicamentos.FirstOrDefaultAsync(m => m.IdMedicamento == id);

            if (medicamento != null)
            {
                this.context.Medicamentos.Remove(medicamento);
                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}