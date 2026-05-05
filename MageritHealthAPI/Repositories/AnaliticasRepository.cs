using MageritHealthAPI.Data;
using MageritHealthAPI.Models;
using MageritHealthAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MageritHealthAPI.Repositories
{
    public class AnaliticasRepository : IAnaliticasRepository
    {
        private readonly MageritHealthDbContext context;

        public AnaliticasRepository(MageritHealthDbContext context)
        {
            this.context = context;
        }

        // --- LECTURA (GET) ---

        public async Task<Analitica> FindAnaliticaByIdAsync(int idAnalitica)
        {
            return await this.context.Analiticas
                .Include(a => a.Cita)
                    .ThenInclude(c => c.Paciente)
                .Include(a => a.Cita)
                    .ThenInclude(c => c.Doctor)
                        .ThenInclude(d => d.Especialidad)
                .FirstOrDefaultAsync(a => a.IdAnalitica == idAnalitica);
        }

        public async Task<List<Analitica>> GetAnaliticasByIdPacienteAsync(int idPaciente)
        {
            return await this.context.Analiticas
                .Include(a => a.Cita)
                    .ThenInclude(c => c.Doctor)
                .Include(a => a.Cita)
                    .ThenInclude(c => c.Paciente)
                .AsNoTracking()
                .Where(a => a.Cita.IdPaciente == idPaciente)
                .OrderByDescending(a => a.FechaAnalitica)
                .ToListAsync();
        }

        public async Task<List<Analitica>> GetAnaliticasByIdDoctorAsync(int idDoctor)
        {
            return await this.context.Analiticas
                .Include(a => a.Cita)
                    .ThenInclude(c => c.Doctor)
                .Include(a => a.Cita)
                    .ThenInclude(c => c.Paciente)
                .AsNoTracking()
                .Where(a => a.Cita.IdDoctor == idDoctor)
                .OrderByDescending(a => a.FechaAnalitica)
                .ToListAsync();
        }

        // --- ESCRITURA (POST / PUT) ---

        public async Task<bool> CreateAnaliticaAsync(Analitica analitica)
        {
            try
            {
                int maxId = await this.context.Analiticas.MaxAsync(a => (int?)a.IdAnalitica) ?? 0;
                analitica.IdAnalitica = maxId + 1;
                analitica.FechaCreacion = DateTime.UtcNow;
                analitica.Estado = "pendiente";

                await this.context.Analiticas.AddAsync(analitica);
                return await this.context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateFechaAnaliticaAsync(int idAnalitica, DateTime nuevaFecha)
        {
            Analitica analitica = await this.context.Analiticas.FirstOrDefaultAsync(a => a.IdAnalitica == idAnalitica);

            if (analitica != null)
            {
                analitica.FechaAnalitica = nuevaFecha;
                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> UpdateEstadoAnaliticaAsync(int idAnalitica, string estado)
        {
            Analitica analitica = await this.context.Analiticas.FirstOrDefaultAsync(a => a.IdAnalitica == idAnalitica);

            if (analitica != null)
            {
                analitica.Estado = estado;
                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> FinalizarAnaliticaAsync(int idAnalitica, List<Medicion> mediciones)
        {
            Analitica analitica = await this.context.Analiticas.FirstOrDefaultAsync(a => a.IdAnalitica == idAnalitica);

            if (analitica != null)
            {
                int maxIdMedicion = await this.context.Mediciones.MaxAsync(m => (int?)m.IdMedicion) ?? 0;

                foreach (Medicion medicion in mediciones)
                {
                    medicion.IdMedicion = maxIdMedicion + 1;
                    medicion.IdAnalitica = idAnalitica;
                    await this.context.Mediciones.AddAsync(medicion);
                    maxIdMedicion++;
                }

                analitica.Estado = "completada";
                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<List<Medicion>> GetListaMedicionesByIdAnaliticaAsync(int idAnalitica)
        {
            return await this.context.Mediciones
                .Include(m => m.TipoMedicion)
                .AsNoTracking()
                .Where(m => m.IdAnalitica == idAnalitica)
                .ToListAsync();
        }
    }
}