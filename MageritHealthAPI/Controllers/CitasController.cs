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
    public class CitasController : ControllerBase
    {
        private readonly ICitasRepository citasRepository;
        private readonly UserTokenHelper userTokenHelper;

        public CitasController(ICitasRepository citasRepository, UserTokenHelper userTokenHelper)
        {
            this.citasRepository = citasRepository;
            this.userTokenHelper = userTokenHelper;
        }

        #region MAPPERS (Traducciones a DTOs)
        private ListCitasModel MapToResumen(Cita cita)
        {
            string apellidoDoc = cita.Doctor?.Apellido2 != null ? $"{cita.Doctor.Apellido1} {cita.Doctor.Apellido2}" : cita.Doctor?.Apellido1 ?? "";
            string apellidoPac = cita.Paciente?.Apellido2 != null ? $"{cita.Paciente.Apellido1} {cita.Paciente.Apellido2}" : cita.Paciente?.Apellido1 ?? "";

            return new ListCitasModel
            {
                IdCita = cita.IdCita,
                Motivo = cita.Motivo,
                FechaHora = cita.FechaHora,
                Estado = cita.Estado,
                NombreDoctor = $"Dr/a. {cita.Doctor?.Nombre} {apellidoDoc}".Trim(),
                EspecialidadDoctor = cita.Doctor?.Especialidad?.NombreEspecialidad ?? "General",
                NombrePaciente = $"{cita.Paciente?.Nombre} {apellidoPac}".Trim(),
                DniPaciente = cita.Paciente?.Dni ?? ""
            };
        }

        private DetailsCitaModel MapToDetalles(Cita cita)
        {
            string apellidoDoc = cita.Doctor?.Apellido2 != null ? $"{cita.Doctor.Apellido1} {cita.Doctor.Apellido2}" : cita.Doctor?.Apellido1 ?? "";
            string apellidoPac = cita.Paciente?.Apellido2 != null ? $"{cita.Paciente.Apellido1} {cita.Paciente.Apellido2}" : cita.Paciente?.Apellido1 ?? "";

            return new DetailsCitaModel
            {
                IdCita = cita.IdCita,
                Motivo = cita.Motivo,
                FechaHora = cita.FechaHora,
                Notas = cita.Notas,
                Estado = cita.Estado,
                NombreDoctor = $"Dr/a. {cita.Doctor?.Nombre} {apellidoDoc}".Trim(),
                Especialidad = cita.Doctor?.Especialidad?.NombreEspecialidad ?? "General",
                NumeroColegiado = cita.Doctor?.NumeroColegiado,
                IdPaciente = cita.IdPaciente,
                NombrePaciente = $"{cita.Paciente?.Nombre} {apellidoPac}".Trim(),
                DniPaciente = cita.Paciente?.Dni ?? ""
            };
        }
        #endregion

        #region GETs (Lectura)
        [HttpGet]
        public async Task<ActionResult<List<ListCitasModel>>> GetAllCitas()
        {
            try
            {
                List<Cita> citas = await this.citasRepository.GetCitasAsync();
                return Ok(citas.Select(MapToResumen).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno.", detalle = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DetailsCitaModel>> GetCitaById(int id)
        {
            try
            {
                Cita cita = await this.citasRepository.FindCitaByIdAsync(id);
                if (cita == null) return NotFound(new { mensaje = "Cita no encontrada." });

                return Ok(this.MapToDetalles(cita));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno.", detalle = ex.Message });
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<ListCitasModel>>> MisProximasCitas([FromQuery] DateTime? fecha)
        {
            try
            {
                var userInfo = this.userTokenHelper.GetInfoUser();
                int myId = int.Parse(userInfo.IdUsuario);
                string myRol = userInfo.Rol.ToLower();

                List<Cita> citas = new List<Cita>();

                if (myRol == "paciente") citas = await this.citasRepository.GetProximasCitasByIdPacienteAsync(myId, fecha);
                else if (myRol == "doctor") citas = await this.citasRepository.GetProximasCitasByIdDoctorAsync(myId, fecha);
                else return Forbid();

                return Ok(citas.Select(MapToResumen).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno.", detalle = ex.Message });
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<ListCitasModel>>> MiHistorial()
        {
            try
            {
                var userInfo = this.userTokenHelper.GetInfoUser();
                int myId = int.Parse(userInfo.IdUsuario);
                string myRol = userInfo.Rol.ToLower();

                List<Cita> citas = new List<Cita>();

                if (myRol == "paciente") citas = await this.citasRepository.GetHistorialCitasByIdPacienteAsync(myId);
                else if (myRol == "doctor") citas = await this.citasRepository.GetHistorialCitasByIdDoctorAsync(myId);
                else return Forbid();

                return Ok(citas.Select(MapToResumen).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno.", detalle = ex.Message });
            }
        }

        [HttpGet]
        [Route("[action]/{idDoctor:int}")]
        public async Task<ActionResult<List<string>>> HorasDisponibles(int idDoctor, [FromQuery] DateTime fecha)
        {
            try
            {
                List<string> horas = await this.citasRepository.GetHorasDisponiblesDoctorAsync(idDoctor, fecha);
                return Ok(horas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno.", detalle = ex.Message });
            }
        }
        #endregion

        #region POST / PUT (Escritura)
        [HttpPost]
        public async Task<ActionResult> CreateCita([FromBody] CreateCitaModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                int idPaciente = model.IdPaciente;
                var userInfo = this.userTokenHelper.GetInfoUser();

                if (userInfo.Rol.ToLower() == "paciente")
                {
                    idPaciente = int.Parse(userInfo.IdUsuario);
                }

                Cita nuevaCita = new Cita
                {
                    IdPaciente = idPaciente,
                    IdDoctor = model.IdDoctor,
                    FechaHora = model.FechaHora,
                    Motivo = model.Motivo,
                    Notas = model.Notas,
                    Estado = "programada",
                    Activa = true
                };

                bool creado = await this.citasRepository.CreateCitaAsync(nuevaCita);

                if (creado)
                {
                    return CreatedAtAction(nameof(GetCitaById), new { id = nuevaCita.IdCita }, new { mensaje = "Cita creada correctamente." });
                }

                return BadRequest(new { mensaje = "No se pudo crear la cita en la base de datos." });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(409, new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno.", detalle = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> EditCita(int id, [FromBody] EditCitaModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var userInfo = this.userTokenHelper.GetInfoUser();
                int userId = int.Parse(userInfo.IdUsuario);
                string userRol = userInfo.Rol.ToLower();

                Cita citaExistente = await this.citasRepository.FindCitaByIdAsync(id);
                if (citaExistente == null) return NotFound(new { mensaje = "Cita no encontrada." });

                if (userRol != "admin")
                {
                    if (userRol == "paciente" && citaExistente.IdPaciente != userId) return Forbid();
                    if (userRol == "doctor" && citaExistente.IdDoctor != userId) return Forbid();
                }

                Cita citaEditada = new Cita
                {
                    IdCita = id,
                    FechaHora = model.NuevaFechaHora,
                    Motivo = model.NuevoMotivo,
                    Notas = model.Notas
                };

                bool actualizado = await this.citasRepository.UpdateCitaAsync(citaEditada);

                if (actualizado) return Ok(new { mensaje = "Cita actualizada correctamente." });

                return NotFound(new { mensaje = "Cita no encontrada o no se pudo actualizar." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno.", detalle = ex.Message });
            }
        }

        [HttpPut]
        [Route("[action]/{id:int}")]
        public async Task<ActionResult> CancelarCita(int id)
        {
            try
            {
                var userInfo = this.userTokenHelper.GetInfoUser();
                int userId = int.Parse(userInfo.IdUsuario);
                string userRol = userInfo.Rol.ToLower();

                Cita citaExistente = await this.citasRepository.FindCitaByIdAsync(id);
                if (citaExistente == null)
                {
                    return NotFound(new { mensaje = "Cita no encontrada." });
                }

                if (userRol != "admin")
                {
                    if (userRol == "paciente" && citaExistente.IdPaciente != userId) return Forbid();
                    if (userRol == "doctor" && citaExistente.IdDoctor != userId) return Forbid();
                }

                bool cancelado = await this.citasRepository.UpdateEstadoCitaAsync(id, "cancelada", false);
                if (cancelado) return Ok(new { mensaje = "Cita cancelada." });

                return NotFound(new { mensaje = "Cita no encontrada." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno.", detalle = ex.Message });
            }
        }

        [Authorize(Roles = "doctor")]
        [HttpPut]
        [Route("[action]/{id:int}")]
        public async Task<ActionResult> IniciarCita(int id)
        {
            try
            {
                bool iniciado = await this.citasRepository.UpdateEstadoCitaAsync(id, "progreso", true);
                if (iniciado) return Ok(new { mensaje = "Cita iniciada." });

                return NotFound(new { mensaje = "Cita no encontrada." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno.", detalle = ex.Message });
            }
        }

        [Authorize(Roles = "doctor")]
        [HttpPut]
        [Route("[action]/{id:int}")]
        public async Task<ActionResult> FinalizarCita(int id)
        {
            try
            {
                bool finalizado = await this.citasRepository.UpdateEstadoCitaAsync(id, "completada", false);
                if (finalizado) return Ok(new { mensaje = "Cita finalizada." });

                return NotFound(new { mensaje = "Cita no encontrada." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno.", detalle = ex.Message });
            }
        }
        #endregion
    }
}