using MageritHealthAPI.Data;
using MageritHealthAPI.Models;
using MageritHealthAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MageritHealthAPI.Repositories
{
    public class AntecedentesRepository : IAntecedentesRepository
    {
        private readonly MageritHealthDbContext context;

        public AntecedentesRepository(MageritHealthDbContext context)
        {
            this.context = context;
        }

        public async Task<List<AntecedenteMedico>> GetAntecedentesByIdPacienteAsync(int idPaciente)
        {
            return await this.context.AntecedentesMedicos
                .AsNoTracking()
                .Where(a => a.IdPaciente == idPaciente)
                .OrderByDescending(a => a.FechaRegistro)
                .ToListAsync();
        }

        public async Task<AntecedenteMedico> GetAntecedenteByIdAsync(int idAntecedente)
        {
            return await this.context.AntecedentesMedicos
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.IdAntecedente == idAntecedente);
        }

        public async Task<bool> CreateAntecedenteAsync(AntecedenteMedico antecedente)
        {
            int maxId = await this.context.AntecedentesMedicos.MaxAsync(a => (int?)a.IdAntecedente) ?? 0;

            antecedente.IdAntecedente = maxId + 1;
            antecedente.Activo = true;
            antecedente.FechaRegistro = DateTime.UtcNow;

            await this.context.AntecedentesMedicos.AddAsync(antecedente);
            return await this.context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAntecedenteAsync(AntecedenteMedico antecedente)
        {
            AntecedenteMedico storedAntecedente = await this.context.AntecedentesMedicos
                .FirstOrDefaultAsync(a => a.IdAntecedente == antecedente.IdAntecedente);

            if (storedAntecedente != null)
            {
                storedAntecedente.Tipo = antecedente.Tipo;
                storedAntecedente.Nombre = antecedente.Nombre;
                storedAntecedente.Severidad = antecedente.Severidad;
                storedAntecedente.FechaDiagnostico = antecedente.FechaDiagnostico;
                storedAntecedente.Notas = antecedente.Notas;

                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> UpdateEstadoAntecedenteAsync(int idAntecedente, bool activo)
        {
            AntecedenteMedico storedAntecedente = await this.context.AntecedentesMedicos
                .FirstOrDefaultAsync(a => a.IdAntecedente == idAntecedente);

            if (storedAntecedente != null)
            {
                storedAntecedente.Activo = activo;
                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> DeleteAntecedenteAsync(int idAntecedente)
        {
            AntecedenteMedico storedAntecedente = await this.context.AntecedentesMedicos
                .FirstOrDefaultAsync(a => a.IdAntecedente == idAntecedente);

            if (storedAntecedente != null)
            {
                this.context.AntecedentesMedicos.Remove(storedAntecedente);
                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}