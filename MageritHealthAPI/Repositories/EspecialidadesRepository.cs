using MageritHealthAPI.Data;
using MageritHealthAPI.Models;
using MageritHealthAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MageritHealthAPI.Repositories
{
    public class EspecialidadesRepository : IEspecialidadesRepository
    {
        private readonly MageritHealthDbContext context;

        public EspecialidadesRepository(MageritHealthDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Especialidad>> GetEspecialidadesAsync()
        {
            return await this.context.Especialidades.ToListAsync();
        }

        public async Task<Especialidad> GetEspecialidadByIdAsync(int id)
        {
            return await this.context.Especialidades.FirstOrDefaultAsync(e => e.IdEspecialidad == id);
        }

        public async Task<bool> CreateEspecialidadAsync(Especialidad especialidad)
        {
            int maxId = await this.context.Especialidades.MaxAsync(e => (int?)e.IdEspecialidad) ?? 0;
            especialidad.IdEspecialidad = maxId + 1;

            await this.context.Especialidades.AddAsync(especialidad);
            return await this.context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateEspecialidadAsync(Especialidad especialidad)
        {
            Especialidad storedEspecialidad = await this.context.Especialidades.FindAsync(especialidad.IdEspecialidad);

            if (storedEspecialidad != null)
            {
                storedEspecialidad.NombreEspecialidad = especialidad.NombreEspecialidad;
                storedEspecialidad.Tipo = especialidad.Tipo;

                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> DeleteEspecialidadAsync(int id)
        {
            var especialidad = await this.context.Especialidades.FindAsync(id);

            if (especialidad != null)
            {
                int doctorCount = await this.context.Usuarios.CountAsync(u => u.IdEspecialidad == id);

                if (doctorCount == 0)
                {
                    this.context.Especialidades.Remove(especialidad);
                    return await this.context.SaveChangesAsync() > 0;
                }
            }
            return false;
        }
    }
}