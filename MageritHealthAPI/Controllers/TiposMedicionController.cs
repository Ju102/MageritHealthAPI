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
    public class TiposMedicionController : ControllerBase
    {
        private readonly ITiposMedicionRepository tiposMedicionRepository;

        public TiposMedicionController(ITiposMedicionRepository tiposMedicionRepository)
        {
            this.tiposMedicionRepository = tiposMedicionRepository;
        }

        #region MAPPERS
        private DetailsTipoMedicionModel MapToDetails(TipoMedicion t)
        {
            return new DetailsTipoMedicionModel
            {
                IdTipoMedicion = t.IdTipoMedicion,
                NombreMedicion = t.NombreMedicion,
                UnidadMedicion = t.UnidadMedicion,
                ValorMaximo = t.ValorMaximo,
                ValorMinimo = t.ValorMinimo,
                Activo = t.Activo
            };
        }
        #endregion

        #region LECTURA (GET)
        [HttpGet]
        public async Task<ActionResult<List<DetailsTipoMedicionModel>>> GetTiposMedicion()
        {
            try
            {
                var tiposMedicion = await tiposMedicionRepository.GetTiposMedicionAsync();
                return Ok(tiposMedicion.Select(MapToDetails).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DetailsTipoMedicionModel>> GetTipoMedicion(int id)
        {
            try
            {
                var tipoMedicion = await tiposMedicionRepository.GetTipoMedicionByIdAsync(id);

                if (tipoMedicion == null)
                {
                    return NotFound(new { mensaje = "Tipo de medición no encontrado." });
                }

                return Ok(MapToDetails(tipoMedicion));
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
        public async Task<ActionResult> Post([FromBody] CreateUpdateTipoMedicionModel model)
        {
            try
            {
                if (model == null || !ModelState.IsValid) return BadRequest(new { mensaje = "Datos inválidos." });

                TipoMedicion tipoMedicion = new TipoMedicion()
                {
                    NombreMedicion = model.NombreMedicion,
                    UnidadMedicion = model.UnidadMedicion,
                    ValorMaximo = model.ValorMaximo,
                    ValorMinimo = model.ValorMinimo,
                    Activo = true
                };

                bool creado = await this.tiposMedicionRepository.CreateTipoMedicionAsync(tipoMedicion);

                if (creado)
                {
                    return Ok(new { mensaje = "Tipo de medición creado." });
                }

                return BadRequest(new { mensaje = "No se ha podido crear el tipo de medición." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] CreateUpdateTipoMedicionModel model)
        {
            try
            {
                if (model == null || !ModelState.IsValid) return BadRequest(new { mensaje = "Datos inválidos." });

                TipoMedicion tipoMedicion = new TipoMedicion()
                {
                    IdTipoMedicion = id,
                    NombreMedicion = model.NombreMedicion,
                    UnidadMedicion = model.UnidadMedicion,
                    ValorMaximo = model.ValorMaximo,
                    ValorMinimo = model.ValorMinimo
                };

                bool actualizado = await this.tiposMedicionRepository.UpdateTipoMedicionAsync(tipoMedicion);

                if (actualizado) return Ok(new { mensaje = "Tipo de medición actualizado correctamente." });

                return NotFound(new { mensaje = "El tipo de medición no existe o no hubo cambios." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("[action]/{id:int}")]
        public async Task<ActionResult> CambiarEstado(int id, [FromBody] bool nuevoEstado)
        {
            try
            {
                bool actualizado = await this.tiposMedicionRepository.UpdateEstadoTipoMedicionAsync(id, nuevoEstado);

                if (actualizado)
                {
                    return Ok(new { mensaje = $"Estado actualizado a {(nuevoEstado ? "Activo" : "Inactivo")}." });
                }

                return NotFound(new { mensaje = "El tipo de medición no existe." });
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
                bool eliminado = await this.tiposMedicionRepository.DeleteTipoMedicionAsync(id);

                if (eliminado) return NoContent();

                return NotFound(new { mensaje = "El tipo de medición que intentas eliminar no existe." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }
        #endregion
    }
}