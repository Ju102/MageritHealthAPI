using MageritHealthAPI.Data;
using MageritHealthAPI.Models;
using MageritHealthAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MageritHealthAPI.Repositories
{
    public class NotificacionesRepository : INotificacionesRepository
    {
        private readonly MageritHealthDbContext context;

        public NotificacionesRepository(MageritHealthDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Notificacion>> GetNotificacionesByIdUsuarioAsync(int idUsuario, bool? soloNoLeidas = null)
        {
            var query = this.context.Notificaciones
                .Where(n => n.IdUsuario == idUsuario)
                .AsNoTracking();

            if (soloNoLeidas.HasValue)
            {
                query = query.Where(n => n.Leido == !soloNoLeidas.Value);
            }

            return await query.OrderByDescending(n => n.FechaCreacion).ToListAsync();
        }

        public async Task<Notificacion> FindNotificacionByIdAsync(int idNotificacion)
        {
            return await this.context.Notificaciones
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.IdNotificacion == idNotificacion);
        }

        public async Task<bool> InsertNotificacionAsync(Notificacion notificacion)
        {
            int lastId = await this.context.Notificaciones.MaxAsync(n => (int?)n.IdNotificacion) ?? 0;
            notificacion.IdNotificacion = lastId + 1;
            notificacion.Leido = false;
            notificacion.FechaCreacion = DateTime.UtcNow;

            await this.context.Notificaciones.AddAsync(notificacion);
            return await this.context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateNotificacionALeidaAsync(int idNotificacion, int idUsuario)
        {
            Notificacion notificacion = await this.context.Notificaciones
                .FirstOrDefaultAsync(n => n.IdNotificacion == idNotificacion && n.IdUsuario == idUsuario);

            if (notificacion != null)
            {
                if (notificacion.Leido) return true;

                notificacion.Leido = true;
                return await this.context.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> MarcarTodasComoLeidasAsync(int idUsuario)
        {
            List<Notificacion> noLeidas = await this.context.Notificaciones
                .Where(n => n.IdUsuario == idUsuario && n.Leido == false)
                .ToListAsync();

            if (noLeidas.Any())
            {
                foreach (Notificacion noti in noLeidas)
                {
                    noti.Leido = true;
                }
                await this.context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> DeleteNotificacionAsync(int idNotificacion, int idUsuario)
        {
            Notificacion notificacion = await this.context.Notificaciones
                .FirstOrDefaultAsync(n => n.IdNotificacion == idNotificacion && n.IdUsuario == idUsuario);

            if (notificacion != null)
            {
                this.context.Notificaciones.Remove(notificacion);
                return await this.context.SaveChangesAsync() > 0;
            }

            return false;
        }
    }
}