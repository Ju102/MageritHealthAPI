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
    public class EspecialidadesController : ControllerBase
    {
        private readonly IEspecialidadesRepository especialidadesRepository;

        public EspecialidadesController(IEspecialidadesRepository especialidadesRepository)
        {
            this.especialidadesRepository = especialidadesRepository;
        }

        #region MAPPERS
        private DetailsEspecialidadModel MapToDetails(Especialidad especialidad)
        {
            return new DetailsEspecialidadModel
            {
                IdEspecialidad = especialidad.IdEspecialidad,
                NombreEspecialidad = especialidad.NombreEspecialidad,
                Tipo = especialidad.Tipo
            };
        }
        #endregion

        #region LECTURA (GET)
        [HttpGet]
        public async Task<ActionResult<List<DetailsEspecialidadModel>>> GetEspecialidades()
        {
            try
            {
                List<Especialidad> especialidades = await this.especialidadesRepository.GetEspecialidadesAsync();

                return Ok(especialidades.Select(MapToDetails).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DetailsEspecialidadModel>> GetEspecialidadById(int id)
        {
            try
            {
                Especialidad especialidad = await this.especialidadesRepository.GetEspecialidadByIdAsync(id);

                if (especialidad == null)
                {
                    return NotFound(new { mensaje = "Especialidad no encontrada." });
                }

                return Ok(MapToDetails(especialidad));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }
        #endregion

        #region ESCRITURA (POST / PUT / DELETE)
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateEditEspecialidadModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.NombreEspecialidad) || string.IsNullOrWhiteSpace(model.Tipo))
                {
                    return BadRequest(new { mensaje = "El nombre y el tipo de la especialidad son obligatorios." });
                }

                Especialidad especialidad = new Especialidad
                {
                    NombreEspecialidad = model.NombreEspecialidad,
                    Tipo = model.Tipo
                };

                bool creado = await this.especialidadesRepository.CreateEspecialidadAsync(especialidad);

                if (creado)
                {
                    return CreatedAtAction(nameof(GetEspecialidadById), new { id = especialidad.IdEspecialidad }, new { mensaje = "Especialidad creada con éxito." });
                }

                return BadRequest(new { mensaje = "No se ha podido guardar la especialidad en la base de datos." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] CreateEditEspecialidadModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.NombreEspecialidad) || string.IsNullOrWhiteSpace(model.Tipo))
                {
                    return BadRequest(new { mensaje = "El nombre y el tipo de la especialidad son obligatorios." });
                }

                Especialidad especialidad = new Especialidad
                {
                    IdEspecialidad = id,
                    NombreEspecialidad = model.NombreEspecialidad,
                    Tipo = model.Tipo
                };

                bool actualizado = await this.especialidadesRepository.UpdateEspecialidadAsync(especialidad);

                if (actualizado)
                {
                    return Ok(new { mensaje = "Especialidad actualizada correctamente." });
                }

                return NotFound(new { mensaje = "Especialidad no encontrada o no hubo cambios." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                bool eliminado = await this.especialidadesRepository.DeleteEspecialidadAsync(id);

                if (eliminado)
                {
                    return Ok(new { mensaje = "Especialidad eliminada correctamente." });
                }

                return NotFound(new { mensaje = "Especialidad no encontrada." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }
        #endregion
    }
}