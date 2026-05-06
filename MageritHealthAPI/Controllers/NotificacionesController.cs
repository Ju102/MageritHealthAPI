using MageritHealthAPI.Helpers;
using MageritHealthAPI.Models;
using MageritHealthAPI.Models.DTOs;
using MageritHealthAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MageritHealthAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacionesController : ControllerBase
    {
        private readonly INotificacionesRepository notificacionesRepository;
        private readonly UserTokenHelper userTokenHelper;

        public NotificacionesController(INotificacionesRepository notificacionesRepository, UserTokenHelper userTokenHelper)
        {
            this.notificacionesRepository = notificacionesRepository;
            this.userTokenHelper = userTokenHelper;
        }

        #region MAPPERS
        private DetailsNotificacionModel MapToDTO(Notificacion n)
        {
            return new DetailsNotificacionModel
            {
                IdNotificacion = n.IdNotificacion,
                Titulo = n.Titulo,
                Mensaje = n.Mensaje,
                Tipo = n.Tipo,
                EnlaceAccion = n.EnlaceAccion,
                Leido = n.Leido,
                FechaCreacion = n.FechaCreacion
            };
        }
        #endregion

        #region LECTURA (GET)
        [HttpGet]
        public async Task<ActionResult<List<DetailsNotificacionModel>>> Get([FromQuery] bool? soloNoLeidas = null)
        {
            try
            {
                int idUsuario = int.Parse(this.userTokenHelper.GetInfoUser().IdUsuario);
                List<Notificacion> notificaciones = await this.notificacionesRepository.GetNotificacionesByIdUsuarioAsync(idUsuario, soloNoLeidas);

                return Ok(notificaciones.Select(MapToDTO).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }
        #endregion

        #region ESCRITURA (POST / PUT / DELETE)
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateNotificacionModel model)
        {
            try
            {
                if (model == null || !ModelState.IsValid) return BadRequest(new { mensaje = "Modelo no válido." });

                Notificacion notificacion = new Notificacion
                {
                    IdUsuario = model.IdUsuario,
                    Titulo = model.Titulo,
                    Mensaje = model.Mensaje,
                    Tipo = model.Tipo,
                    EnlaceAccion = model.EnlaceAccion
                };

                bool creado = await this.notificacionesRepository.InsertNotificacionAsync(notificacion);

                if (creado)
                {
                    return Ok(new { mensaje = "Notificación enviada correctamente." });
                }

                return BadRequest(new { mensaje = "No se pudo guardar la notificación." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("{id:int}/leer")]
        public async Task<ActionResult> MarcarLeida(int id)
        {
            try
            {
                int idUsuario = int.Parse(this.userTokenHelper.GetInfoUser().IdUsuario);

                bool success = await this.notificacionesRepository.UpdateNotificacionALeidaAsync(id, idUsuario);

                if (!success) return NotFound(new { mensaje = "Notificación no encontrada o no pertenece a tu usuario." });

                return Ok(new { mensaje = "Notificación marcada como leída." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<ActionResult> MarcarTodasLeidas()
        {
            try
            {
                int idUsuario = int.Parse(this.userTokenHelper.GetInfoUser().IdUsuario);
                await this.notificacionesRepository.MarcarTodasComoLeidasAsync(idUsuario);

                return Ok(new { mensaje = "Todas las notificaciones marcadas como leídas." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                int idUsuario = int.Parse(this.userTokenHelper.GetInfoUser().IdUsuario);

                bool success = await this.notificacionesRepository.DeleteNotificacionAsync(id, idUsuario);

                if (!success) return NotFound(new { mensaje = "Notificación no encontrada o no pertenece a tu usuario." });

                return Ok(new { mensaje = "Notificación eliminada." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }
        #endregion
    }
}