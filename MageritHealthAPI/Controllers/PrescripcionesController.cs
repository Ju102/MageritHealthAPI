using MageritHealthAPI.Helpers;
using MageritHealthAPI.Models;
using MageritHealthAPI.Models.DTOs;
using MageritHealthAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MageritHealthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrescripcionesController : ControllerBase
    {
        private readonly IPrescripcionesRepository prescripcionesRepository;
        private readonly UserTokenHelper userTokenHelper;

        public PrescripcionesController(IPrescripcionesRepository prescripcionesRepository, UserTokenHelper userTokenHelper)
        {
            this.prescripcionesRepository = prescripcionesRepository;
            this.userTokenHelper = userTokenHelper;
        }

        #region MAPPERS
        private DetailsPrescripcionModel MapToDetails(Prescripcion p)
        {
            return new DetailsPrescripcionModel
            {
                IdPrescripcion = p.IdPrescripcion,
                IdCita = p.IdCita,
                IdMedicamento = p.IdMedicamento,
                Instrucciones = p.Instrucciones,
                FechaInicio = p.FechaInicio,
                FechaFin = p.FechaFin,
                NombreComercial = p.Medicamento?.NombreComercial ?? "Desconocido",
                PrincipioActivo = p.Medicamento?.PrincipioActivo ?? "Desconocido",
                Concentracion = p.Medicamento?.Concentracion ?? "Desconocido"
            };
        }
        #endregion

        #region LECTURA (GET)
        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<ActionResult<DetailsPrescripcionModel>> GetById(int id)
        {
            try
            {
                var prescripcion = await this.prescripcionesRepository.GetPrescripcionByIdAsync(id);

                if (prescripcion == null)
                {
                    return NotFound(new { mensaje = $"No se encontró la prescripción con ID {id}." });
                }

                // Validación de seguridad (IDOR)
                var userInfo = this.userTokenHelper.GetInfoUser();
                int myId = int.Parse(userInfo.IdUsuario);
                string myRol = userInfo.Rol.ToLower();

                if (myRol == "paciente" && prescripcion.Cita?.IdPaciente != myId) return Forbid();
                if (myRol == "doctor" && prescripcion.Cita?.IdDoctor != myId) return Forbid();

                return Ok(this.MapToDetails(prescripcion));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener la prescripción.", detalle = ex.Message });
            }
        }

        [HttpGet]
        [Route("[action]/{idPaciente:int}")]
        public async Task<ActionResult<List<DetailsPrescripcionModel>>> GetPrescripcionesPaciente(int idPaciente)
        {
            try
            {
                var userInfo = this.userTokenHelper.GetInfoUser();
                int userId = int.Parse(userInfo.IdUsuario);
                string userRol = userInfo.Rol.ToLower();

                if (userRol == "paciente" && idPaciente != userId) return Forbid();

                List<Prescripcion> prescripciones = await this.prescripcionesRepository.GetPrescripcionesByPacienteIdAsync(idPaciente);

                return Ok(prescripciones.Select(MapToDetails).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el historial de prescripciones.", detalle = ex.Message });
            }
        }
        #endregion

        #region ESCRITURA (POST / PUT / DELETE)
        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "doctor,admin")]
        public async Task<ActionResult> Create([FromBody] CreateUpdatePrescripcionModel model)
        {
            try
            {
                if (model == null || !ModelState.IsValid) return BadRequest(new { mensaje = "Modelo inválido." });

                Prescripcion prescripcion = new Prescripcion
                {
                    IdCita = model.IdCita,
                    IdMedicamento = model.IdMedicamento,
                    Instrucciones = model.Instrucciones,
                    FechaInicio = model.FechaInicio,
                    FechaFin = model.FechaFin
                };

                bool creado = await this.prescripcionesRepository.CreatePrescripcionAsync(prescripcion);

                if (creado)
                {
                    return CreatedAtAction(nameof(GetById), new { id = prescripcion.IdPrescripcion }, new { mensaje = "Prescripción creada." });
                }

                return BadRequest(new { mensaje = "No se ha podido guardar la prescripción." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear la prescripción.", detalle = ex.Message });
            }
        }

        [HttpPut]
        [Route("[action]/{id:int}")]
        [Authorize(Roles = "doctor,admin")]
        public async Task<ActionResult> Edit(int id, [FromBody] CreateUpdatePrescripcionModel model)
        {
            try
            {
                if (model == null || !ModelState.IsValid) return BadRequest(new { mensaje = "Los datos son inválidos." });

                Prescripcion prescripcion = new Prescripcion
                {
                    IdPrescripcion = id,
                    IdCita = model.IdCita,
                    IdMedicamento = model.IdMedicamento,
                    Instrucciones = model.Instrucciones,
                    FechaInicio = model.FechaInicio,
                    FechaFin = model.FechaFin
                };

                bool actualizado = await this.prescripcionesRepository.UpdatePrescripcionAsync(prescripcion);

                if (actualizado) return Ok(new { mensaje = "Prescripción actualizada correctamente." });

                return NotFound(new { mensaje = "La prescripción que intentas actualizar no existe." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar la prescripción.", detalle = ex.Message });
            }
        }

        [HttpPut]
        [Route("[action]/{id:int}")]
        [Authorize(Roles = "doctor,admin")]
        public async Task<ActionResult> CambiarEstado(int id, [FromBody] bool nuevoEstado)
        {
            try
            {
                bool actualizado = await this.prescripcionesRepository.UpdateEstadoPrescripcionAsync(id, nuevoEstado);

                if (actualizado)
                {
                    return Ok(new { mensaje = $"El estado de la prescripción ha sido actualizado a {(nuevoEstado ? "Activa" : "Inactiva")}." });
                }

                return NotFound(new { mensaje = "La prescripción no existe." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al cambiar el estado de la prescripción.", detalle = ex.Message });
            }
        }

        [HttpDelete]
        [Route("[action]/{id:int}")]
        [Authorize(Roles = "doctor,admin")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                bool eliminado = await this.prescripcionesRepository.DeletePrescripcionAsync(id);

                if (eliminado)
                {
                    return NoContent();
                }

                return NotFound(new { mensaje = "La prescripción que intentas eliminar no existe." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar la prescripción.", detalle = ex.Message });
            }
        }
        #endregion
    }
}