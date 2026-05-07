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
    public class InfoClinicaController : ControllerBase
    {
        private readonly IInfoClinicaRepository infoClinicaRepository;
        private readonly UserTokenHelper userTokenHelper;

        public InfoClinicaController(IInfoClinicaRepository infoClinicaRepository, UserTokenHelper userTokenHelper)
        {
            this.infoClinicaRepository = infoClinicaRepository;
            this.userTokenHelper = userTokenHelper;
        }

        #region MAPPERS
        private DetailsInfoClinicaModel MapToDTO(InfoClinicaPaciente info)
        {
            return new DetailsInfoClinicaModel
            {
                IdPaciente = info.IdPaciente,
                GrupoSanguineo = info.GrupoSanguineo,
                PesoActual = info.PesoActual,
                EstaturaActual = info.EstaturaActual,
                FechaCreacion = info.FechaCreacion,
                FechaActualizacion = info.FechaActualizacion
            };
        }
        #endregion

        #region LECTURA (GET)
        [HttpGet]
        [Route("[action]/{idPaciente:int}")]
        public async Task<ActionResult<DetailsInfoClinicaModel>> GetByPaciente(int idPaciente)
        {
            try
            {
                var userInfo = this.userTokenHelper.GetInfoUser();
                int userId = int.Parse(userInfo.IdUsuario);
                string userRol = userInfo.Rol.ToLower();

                if (userRol == "paciente" && idPaciente != userId) return Forbid();

                InfoClinicaPaciente info = await this.infoClinicaRepository.GetInfoClinicaByIdPacienteAsync(idPaciente);

                if (info == null) return NotFound(new { mensaje = "El paciente no tiene información clínica registrada." });

                return Ok(MapToDTO(info));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }
        #endregion

        #region ESCRITURA (POST / PUT)
        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "doctor,admin")]
        public async Task<ActionResult> Create([FromBody] CreateUpdateInfoClinicaModel model)
        {
            try
            {
                if (model == null || !ModelState.IsValid) return BadRequest(new { mensaje = "Datos inválidos." });

                InfoClinicaPaciente infoClinica = new InfoClinicaPaciente
                {
                    IdPaciente = model.IdPaciente,
                    GrupoSanguineo = model.GrupoSanguineo,
                    PesoActual = model.PesoActual,
                    EstaturaActual = model.EstaturaActual
                };

                bool creado = await this.infoClinicaRepository.CreateInfoClinicaAsync(infoClinica);

                if (creado)
                {
                    return Ok(new { mensaje = "Información clínica creada." });
                }

                return BadRequest(new { mensaje = "El paciente ya tiene una ficha clínica o hubo un error al guardarla." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("[action]/{idPaciente:int}")]
        [Authorize(Roles = "doctor,admin")]
        public async Task<ActionResult> Edit(int idPaciente, [FromBody] CreateUpdateInfoClinicaModel model)
        {
            try
            {
                if (model == null || !ModelState.IsValid) return BadRequest(new { mensaje = "Datos inválidos." });

                // Al DTO le inyectamos el IdPaciente de la ruta
                InfoClinicaPaciente infoClinica = new InfoClinicaPaciente
                {
                    IdPaciente = idPaciente,
                    GrupoSanguineo = model.GrupoSanguineo,
                    PesoActual = model.PesoActual,
                    EstaturaActual = model.EstaturaActual
                };

                bool actualizado = await this.infoClinicaRepository.UpdateInfoClinicaAsync(infoClinica);

                if (actualizado) return Ok(new { mensaje = "Información clínica actualizada correctamente." });

                return NotFound(new { mensaje = "El paciente no tiene una ficha clínica previa para actualizar." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }
        #endregion
    }
}