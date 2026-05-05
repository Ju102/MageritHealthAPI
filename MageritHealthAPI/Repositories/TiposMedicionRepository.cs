using MageritHealthAPI.Data;
using MageritHealthAPI.Models;
using MageritHealthAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MageritHealthAPI.Repositories
{
    public class TiposMedicionRepository : ITiposMedicionRepository
    {
        private readonly MageritHealthDbContext context;

        public TiposMedicionRepository(MageritHealthDbContext context)
        {
            this.context = context;
        }

        public async Task<List<TipoMedicion>> GetTiposMedicionAsync()
        {
            return await this.context.TiposMedicion.AsNoTracking().ToListAsync();
        }

        public async Task<TipoMedicion> GetTipoMedicionByIdAsync(int id)
        {
            return await this.context.TiposMedicion.AsNoTracking().FirstOrDefaultAsync(t => t.IdTipoMedicion == id);
        }

        public async Task<bool> CreateTipoMedicionAsync(TipoMedicion tipoMedicion)
        {
            int maxId = await this.context.TiposMedicion.MaxAsync(t => (int?)t.IdTipoMedicion) ?? 0;

            tipoMedicion.IdTipoMedicion = maxId + 1;
            tipoMedicion.Activo = true;

            await this.context.TiposMedicion.AddAsync(tipoMedicion);
            return await this.context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateTipoMedicionAsync(TipoMedicion tipoMedicion)
        {
            TipoMedicion existingTipoMedicion = await this.context.TiposMedicion.FirstOrDefaultAsync(t => t.IdTipoMedicion == tipoMedicion.IdTipoMedicion);

            if (existingTipoMedicion != null)
            {
                existingTipoMedicion.NombreMedicion = tipoMedicion.NombreMedicion;
                existingTipoMedicion.UnidadMedicion = tipoMedicion.UnidadMedicion;
                existingTipoMedicion.ValorMaximo = tipoMedicion.ValorMaximo;
                existingTipoMedicion.ValorMinimo = tipoMedicion.ValorMinimo;

                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> UpdateEstadoTipoMedicionAsync(int id, bool activo)
        {
            TipoMedicion existingTipoMedicion = await this.context.TiposMedicion.FirstOrDefaultAsync(t => t.IdTipoMedicion == id);

            if (existingTipoMedicion != null)
            {
                existingTipoMedicion.Activo = activo;
                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> DeleteTipoMedicionAsync(int id)
        {
            TipoMedicion tipoMedicion = await this.context.TiposMedicion.FirstOrDefaultAsync(t => t.IdTipoMedicion == id);

            if (tipoMedicion != null)
            {
                int medicionesCount = await this.context.Mediciones.Where(m => m.IdTipoMedicion == id).CountAsync();

                if (medicionesCount > 0)
                {
                    return false;
                }

                this.context.TiposMedicion.Remove(tipoMedicion);
                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}