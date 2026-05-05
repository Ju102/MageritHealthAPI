using MageritHealthAPI.Models;

namespace MageritHealthAPI.Repositories.Interfaces
{
    public interface INotificacionesRepository
    {
        Task<List<Notificacion>> GetNotificacionesByIdUsuarioAsync(int idUsuario, bool? soloNoLeidas = null);

        Task<Notificacion> FindNotificacionByIdAsync(int idNotificacion);

        Task<bool> MarcarTodasComoLeidasAsync(int idUsuario);

        Task<bool> InsertNotificacionAsync(Notificacion notificacion);

        Task<bool> UpdateNotificacionALeidaAsync(int idNotificacion, int idUsuario);

        Task<bool> DeleteNotificacionAsync(int idNotificacion, int idUsuario);
    }
}
