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
    public class AntecedentesController : ControllerBase
    {
        private readonly IAntecedentesRepository antecedentesRepository;
        private readonly UserTokenHelper userTokenHelper;

        public AntecedentesController(IAntecedentesRepository antecedentesRepository, UserTokenHelper userTokenHelper)
        {
            this.antecedentesRepository = antecedentesRepository;
            this.userTokenHelper = userTokenHelper;
        }

        #region MAPPERS
        private DetailsAntecedenteModel MapToDTO(AntecedenteMedico a)
        {
            return new DetailsAntecedenteModel
            {
                IdAntecedente = a.IdAntecedente,
                Tipo = a.Tipo,
                Nombre = a.Nombre,
                Severidad = a.Severidad,
                FechaDiagnostico = a.FechaDiagnostico,
                Notas = a.Notas,
                Activo = a.Activo
            };
        }
        #endregion

        #region LECTURA (GET)
        [HttpGet]
        [Route("[action]/{idPaciente:int}")]
        public async Task<ActionResult<List<DetailsAntecedenteModel>>> GetHistorialPaciente(int idPaciente)
        {
            try
            {
                var userInfo = this.userTokenHelper.GetInfoUser();
                int userId = int.Parse(userInfo.IdUsuario);
                string userRol = userInfo.Rol.ToLower();

                if (userRol == "paciente" && idPaciente != userId) return Forbid();

                List<AntecedenteMedico> antecedentes = await this.antecedentesRepository.GetAntecedentesByIdPacienteAsync(idPaciente);

                return Ok(antecedentes.Select(MapToDTO).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }
        #endregion

        #region ESCRITURA (POST / PUT / DELETE)
        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "doctor,admin")]
        public async Task<ActionResult> Create([FromBody] CreateAntecedenteModel model)
        {
            try
            {
                if (model == null || !ModelState.IsValid) return BadRequest(new { mensaje = "Datos inválidos." });

                AntecedenteMedico antecedente = new AntecedenteMedico
                {
                    IdPaciente = model.IdPaciente,
                    Tipo = model.Tipo,
                    Nombre = model.Nombre,
                    Severidad = model.Severidad,
                    FechaDiagnostico = model.FechaDiagnostico,
                    Notas = model.Notas
                };

                bool creado = await this.antecedentesRepository.CreateAntecedenteAsync(antecedente);

                if (creado)
                {
                    return CreatedAtAction(nameof(GetHistorialPaciente), new { mensaje = "Antecedente registrado." });
                }

                return BadRequest(new { mensaje = "No se ha podido guardar el antecedente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("[action]/{idAntecedente:int}")]
        [Authorize(Roles = "doctor,admin")]
        public async Task<ActionResult> Edit(int idAntecedente, [FromBody] UpdateAntecedenteModel model)
        {
            try
            {
                if (model == null || !ModelState.IsValid) return BadRequest(new { mensaje = "Datos inválidos." });

                AntecedenteMedico antecedente = new AntecedenteMedico
                {
                    IdAntecedente = idAntecedente,
                    Tipo = model.Tipo,
                    Nombre = model.Nombre,
                    Severidad = model.Severidad,
                    FechaDiagnostico = model.FechaDiagnostico,
                    Notas = model.Notas
                };

                bool actualizado = await this.antecedentesRepository.UpdateAntecedenteAsync(antecedente);

                if (actualizado) return Ok(new { mensaje = "Antecedente actualizado correctamente." });

                return NotFound(new { mensaje = "El antecedente no existe." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("[action]/{idAntecedente:int}")]
        [Authorize(Roles = "doctor,admin")]
        public async Task<ActionResult> CambiarEstado(int idAntecedente, [FromBody] bool activo)
        {
            try
            {
                bool actualizado = await this.antecedentesRepository.UpdateEstadoAntecedenteAsync(idAntecedente, activo);

                if (actualizado)
                {
                    return Ok(new { mensaje = $"El antecedente se ha marcado como {(activo ? "Activo" : "Inactivo")}." });
                }

                return NotFound(new { mensaje = "El antecedente no existe." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpDelete]
        [Route("[action]/{idAntecedente:int}")]
        [Authorize(Roles = "doctor,admin")]
        public async Task<ActionResult> Delete(int idAntecedente)
        {
            try
            {
                bool eliminado = await this.antecedentesRepository.DeleteAntecedenteAsync(idAntecedente);

                if (eliminado) return NoContent();

                return NotFound(new { mensaje = "El antecedente que intentas eliminar no existe." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }
        #endregion
    }
}