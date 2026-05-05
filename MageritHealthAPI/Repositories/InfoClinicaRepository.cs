using MageritHealthAPI.Data;
using MageritHealthAPI.Models;
using MageritHealthAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MageritHealthAPI.Repositories
{
    public class InfoClinicaRepository : IInfoClinicaRepository
    {
        private readonly MageritHealthDbContext context;

        public InfoClinicaRepository(MageritHealthDbContext context)
        {
            this.context = context;
        }

        public async Task<InfoClinicaPaciente> GetInfoClinicaByIdPacienteAsync(int idPaciente)
        {
            return await this.context.InfoClinicaPacientes
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.IdPaciente == idPaciente);
        }

        public async Task<bool> CreateInfoClinicaAsync(InfoClinicaPaciente infoClinica)
        {
            bool existe = await this.context.InfoClinicaPacientes.AnyAsync(i => i.IdPaciente == infoClinica.IdPaciente);
            if (existe) return false;

            int maxId = await this.context.InfoClinicaPacientes.MaxAsync(i => (int?)i.IdInfoClinica) ?? 0;

            infoClinica.IdInfoClinica = maxId + 1;
            infoClinica.FechaCreacion = DateTime.UtcNow;
            infoClinica.FechaActualizacion = DateTime.UtcNow;

            await this.context.InfoClinicaPacientes.AddAsync(infoClinica);
            return await this.context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateInfoClinicaAsync(InfoClinicaPaciente infoClinica)
        {
            InfoClinicaPaciente storedInfo = await this.context.InfoClinicaPacientes
                .FirstOrDefaultAsync(i => i.IdPaciente == infoClinica.IdPaciente);

            if (storedInfo != null)
            {
                storedInfo.GrupoSanguineo = infoClinica.GrupoSanguineo;
                storedInfo.PesoActual = infoClinica.PesoActual;
                storedInfo.EstaturaActual = infoClinica.EstaturaActual;

                // Automatizamos la fecha de actualización
                storedInfo.FechaActualizacion = DateTime.UtcNow;

                return await this.context.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}