using MageritHealthAPI.Helpers;
using MageritHealthAPI.Models;
using MageritHealthAPI.Models.DTOs;
using MageritHealthAPI.Repositories.Interfaces;
using MageritHealthAPI.Services.Interfaces;
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
        private readonly IExportService exportService;

        public PrescripcionesController(IPrescripcionesRepository prescripcionesRepository, UserTokenHelper userTokenHelper, IExportService exportService)
        {
            this.prescripcionesRepository = prescripcionesRepository;
            this.userTokenHelper = userTokenHelper;
            this.exportService = exportService;
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

                var userInfo = this.userTokenHelper.GetInfoUser();
                int userId = int.Parse(userInfo.IdUsuario);
                string userRol = userInfo.Rol.ToLower();

                if (userRol == "paciente" && prescripcion.Cita?.IdPaciente != userId) return Forbid();
                if (userRol == "doctor" && prescripcion.Cita?.IdDoctor != userId) return Forbid();

                return Ok(this.MapToDetails(prescripcion));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<ActionResult<List<DetailsPrescripcionModel>>> PrescripcionesByIdCita(int id)
        {
            try
            {
                List<Prescripcion> prescripciones = await this.prescripcionesRepository.GetPrescripcionesByIdCitaAsync(id);

                if (prescripciones == null)
                {
                    return NotFound(new { mensaje = $"No se encontraron las prescripciones con IdCita {id}." });
                }

                var userInfo = this.userTokenHelper.GetInfoUser();
                int userId = int.Parse(userInfo.IdUsuario);
                string userRol = userInfo.Rol.ToLower();

                if (userRol == "paciente" && prescripciones[0].Cita?.IdPaciente != userId) return Forbid();
                if (userRol == "doctor" && prescripciones[0].Cita?.IdDoctor != userId) return Forbid();

                return Ok(prescripciones.Select(MapToDetails).ToList());

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("[action]/{idCita:int}")]
        public async Task<IActionResult> DescargarRecetaPdf(int idCita)
        {
            try
            {
                List<Prescripcion> prescripciones = await this.prescripcionesRepository.GetPrescripcionesByIdCitaAsync(idCita);

                if (prescripciones == null || !prescripciones.Any())
                {
                    return NotFound(new { mensaje = "No hay medicamentos prescritos en esta cita." });
                }

                var userInfo = this.userTokenHelper.GetInfoUser();
                int userId = int.Parse(userInfo.IdUsuario);
                string userRol = userInfo.Rol.ToLower();

                var cita = prescripciones.First().Cita;

                if (userRol == "paciente" && cita?.IdPaciente != userId) return Forbid();

                if (userRol == "doctor" && cita?.IdDoctor != userId) return Forbid();

                byte[] pdfData = await this.exportService.GenerarRecetasPorCitaPdfAsync(idCita);

                if (pdfData == null)
                {
                    return StatusCode(500, new { mensaje = "Hubo un error al generar el documento de la receta." });
                }

                string fileName = $"Receta_Medica_Cita_{idCita}_{DateTime.Now.ToString("yyyyMMdd")}.pdf";
                return File(pdfData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
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

                List<Prescripcion> prescripciones = await this.prescripcionesRepository.GetPrescripcionesByIdPacienteAsync(idPaciente);

                return Ok(prescripciones.Select(MapToDetails).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [Authorize(Roles = "doctor, admin")]
        [HttpGet]
        [Route("[action]/{idDoctor:int}")]
        public async Task<ActionResult<List<DetailsPrescripcionModel>>> GetPrescripcionesDoctor(int idDoctor)
        {
            try
            {
                var userInfo = this.userTokenHelper.GetInfoUser();
                int userId = int.Parse(userInfo.IdUsuario);
                string userRol = userInfo.Rol.ToLower();

                if (userRol == "paciente" || userRol == "doctor" && idDoctor != userId) return Forbid();

                List<Prescripcion> prescripciones = await this.prescripcionesRepository.GetPrescripcionesByIdDoctorAsync(idDoctor);

                return Ok(prescripciones.Select(MapToDetails).ToList());
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
                    return CreatedAtAction(nameof(GetById), new { mensaje = "Prescripción creada." });
                }

                return BadRequest(new { mensaje = "No se ha podido guardar la prescripción." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
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
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
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
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
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
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }
        #endregion
    }
}