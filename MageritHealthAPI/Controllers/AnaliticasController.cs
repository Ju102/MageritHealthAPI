using MageritHealthAPI.Helpers;
using MageritHealthAPI.Models;
using MageritHealthAPI.Models.DTOs;
using MageritHealthAPI.Repositories.Interfaces;
using MageritHealthAPI.Services;
using MageritHealthAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MageritHealthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AnaliticasController : ControllerBase
    {
        private readonly IAnaliticasRepository analiticasRepository;
        private readonly ITiposMedicionRepository tiposMedicionRepository;
        private readonly IExportService exportService;
        private readonly SimuladorLaboratorioService simuladorService;
        private readonly UserTokenHelper userTokenHelper;
        private readonly IAzureBlobService azureBlobService;
        private readonly IConfiguration configuration;

        public AnaliticasController(IAnaliticasRepository analiticasRepository, ITiposMedicionRepository tiposMedicionRepository,
            IExportService exportService, UserTokenHelper userTokenHelper, SimuladorLaboratorioService simuladorService,
            IConfiguration configuration, IAzureBlobService azureBlobService)
        {
            this.analiticasRepository = analiticasRepository;
            this.tiposMedicionRepository = tiposMedicionRepository;
            this.exportService = exportService;
            this.userTokenHelper = userTokenHelper;
            this.simuladorService = simuladorService;
            this.configuration = configuration;
            this.azureBlobService = azureBlobService;
        }

        #region MAPPERS
        private ListAnaliticasModel MapToListModel(Analitica analitica)
        {
            string apellidoDoc = analitica.Cita?.Doctor?.Apellido2 != null ? $"{analitica.Cita.Doctor.Apellido1} {analitica.Cita.Doctor.Apellido2}" : analitica.Cita?.Doctor?.Apellido1 ?? "";
            string apellidoPac = analitica.Cita?.Paciente?.Apellido2 != null ? $"{analitica.Cita.Paciente.Apellido1} {analitica.Cita.Paciente.Apellido2}" : analitica.Cita?.Paciente?.Apellido1 ?? "";

            return new ListAnaliticasModel
            {
                IdAnalitica = analitica.IdAnalitica,
                FechaHora = analitica.FechaAnalitica,
                Estado = analitica.Estado,
                NombreDoctor = $"Dr/a. {analitica.Cita?.Doctor?.Nombre} {apellidoDoc}".Trim(),
                NombrePaciente = $"{analitica.Cita?.Paciente?.Nombre} {apellidoPac}".Trim()
            };
        }

        private DetailsAnaliticaModel MapToDetallesModel(Analitica analitica)
        {
            string apellidoDoc = analitica.Cita?.Doctor?.Apellido2 != null ? $"{analitica.Cita.Doctor.Apellido1} {analitica.Cita.Doctor.Apellido2}" : analitica.Cita?.Doctor?.Apellido1 ?? "";
            string apellidoPac = analitica.Cita?.Paciente?.Apellido2 != null ? $"{analitica.Cita.Paciente.Apellido1} {analitica.Cita.Paciente.Apellido2}" : analitica.Cita?.Paciente?.Apellido1 ?? "";

            return new DetailsAnaliticaModel
            {
                IdAnalitica = analitica.IdAnalitica,
                FechaHora = analitica.FechaAnalitica,
                Notas = analitica.Notas,
                Estado = analitica.Estado,
                IdCita = analitica.IdCita,
                MotivoCita = analitica.Cita?.Motivo ?? "Sin motivo especificado",
                NombrePaciente = $"{analitica.Cita?.Paciente?.Nombre} {apellidoPac}".Trim(),
                DniPaciente = analitica.Cita?.Paciente?.Dni ?? "",
                NombreDoctor = $"Dr/a. {analitica.Cita?.Doctor?.Nombre} {apellidoDoc}".Trim(),
                NumeroColegiado = analitica.Cita?.Doctor?.NumeroColegiado ?? ""
            };
        }
        #endregion

        #region GETs (Lectura)
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<ListAnaliticasModel>>> MisAnaliticas()
        {
            try
            {
                var userInfo = this.userTokenHelper.GetInfoUser();
                int myId = int.Parse(userInfo.IdUsuario);
                string myRol = userInfo.Rol.ToLower();

                List<Analitica> analiticas = new List<Analitica>();

                if (myRol == "paciente")
                {
                    analiticas = await this.analiticasRepository.GetAnaliticasByIdPacienteAsync(myId);
                }
                else if (myRol == "doctor")
                {
                    analiticas = await this.analiticasRepository.GetAnaliticasByIdDoctorAsync(myId);
                }
                else
                {
                    return Forbid();
                }

                return Ok(analiticas.Select(MapToListModel).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [Authorize(Roles = "doctor, admin")]
        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<ActionResult<List<ListAnaliticasModel>>> AnaliticasDoctor(int id)
        {
            try
            {
                var userInfo = this.userTokenHelper.GetInfoUser();
                int userId = int.Parse(userInfo.IdUsuario);
                string userRol = userInfo.Rol.ToLower();

                List<Analitica> analiticas = new List<Analitica>();

                if (userRol == "paciente" || (userRol == "doctor" && userId != id))
                {
                    return Forbid();
                }
                else
                {
                    analiticas = await this.analiticasRepository.GetAnaliticasByIdDoctorAsync(userId);
                }

                return Ok(analiticas.Select(MapToListModel).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<ActionResult<List<ListAnaliticasModel>>> AnaliticasPaciente(int id)
        {
            try
            {
                var userInfo = this.userTokenHelper.GetInfoUser();
                int userId = int.Parse(userInfo.IdUsuario);
                string userRol = userInfo.Rol.ToLower();

                List<Analitica> analiticas = new List<Analitica>();

                if (userRol == "paciente" && userId != id)
                {
                    return Forbid();
                }
                else
                {
                    analiticas = await this.analiticasRepository.GetAnaliticasByIdPacienteAsync(userId);
                }

                return Ok(analiticas.Select(MapToListModel).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<DetailsAnaliticaModel>> GetAnaliticaById(int id)
        {
            try
            {
                Analitica analitica = await this.analiticasRepository.FindAnaliticaByIdAsync(id);

                if (analitica == null) return NotFound(new { mensaje = "Analítica no encontrada." });

                return Ok(this.MapToDetallesModel(analitica));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("[action]/{id:int}")]
        public async Task<ActionResult<List<ListMedicionesModel>>> GetMediciones(int id)
        {
            try
            {
                List<Medicion> mediciones = await this.analiticasRepository.GetListaMedicionesByIdAnaliticaAsync(id);

                List<ListMedicionesModel> medicionesModel = new List<ListMedicionesModel>();
                foreach (Medicion medicion in mediciones)
                {
                    medicionesModel.Add(new ListMedicionesModel
                    {
                        NombreMedicion = medicion.TipoMedicion?.NombreMedicion ?? "Desconocida",
                        UnidadMedicion = medicion.TipoMedicion?.UnidadMedicion ?? "",
                        ValorMaximo = medicion.TipoMedicion?.ValorMaximo ?? 0,
                        ValorMinimo = medicion.TipoMedicion?.ValorMinimo ?? 0,
                        ValorMedicion = medicion.ValorMedicion
                    });
                }

                return Ok(medicionesModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }
        #endregion

        #region POST / PUT (Escritura)
        [HttpPost]
        [Authorize(Roles = "doctor,admin")]
        public async Task<ActionResult> CreateAnalitica([FromBody] CreateAnaliticaModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                Analitica nuevaAnalitica = new Analitica
                {
                    IdCita = model.IdCita,
                    FechaAnalitica = model.FechaHora,
                    Notas = model.Notas
                };

                bool creado = await this.analiticasRepository.CreateAnaliticaAsync(nuevaAnalitica);

                if (creado)
                {
                    return Ok(new { mensaje = "Analítica solicitada correctamente." });
                }
                else
                {
                    return BadRequest(new { mensaje = "No se pudo crear la analítica en la base de datos." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("[action]/{id:int}")]
        public async Task<ActionResult> UpdateFecha(int id, DateTime nuevaFecha)
        {
            try
            {
                bool actualizado = await this.analiticasRepository.UpdateFechaAnaliticaAsync(id, nuevaFecha);

                if (actualizado)
                {
                    return Ok(new { mensaje = "Fecha de la analítica actualizada." });
                }
                else
                {
                    return NotFound(new { mensaje = "Analítica no encontrada o no se pudo actualizar." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("[action]/{id:int}")]
        public async Task<ActionResult> CancelarAnalitica(int id)
        {
            try
            {
                bool cancelado = await this.analiticasRepository.UpdateEstadoAnaliticaAsync(id, "cancelada");

                if (cancelado)
                {
                    return Ok(new { mensaje = "Analítica cancelada." });
                }
                else
                {
                    return NotFound(new { mensaje = "Analítica no encontrada o no se pudo cancelar." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("[action]/{id:int}")]
        [Authorize(Roles = "doctor,admin")]
        public async Task<ActionResult> FinalizarAnalitica(int id)
        {
            try
            {
                List<TipoMedicion> tiposMedicion = await this.tiposMedicionRepository.GetTiposMedicionAsync();
                List<Medicion> mockMediciones = this.simuladorService.GenerarMedicionesCasuales(tiposMedicion, id);

                bool finalizado = await this.analiticasRepository.FinalizarAnaliticaAsync(id, mockMediciones);

                if (finalizado)
                {
                    byte[] pdfData = await this.exportService.GenerarInformeAnaliticaPdfAsync(id);

                    if (pdfData != null)
                    {
                        string container = this.configuration["AzureStorageConfig:ContainerPDFs"];
                        string fileName = $"resultado_analitica_{id}_{DateTime.UtcNow.Ticks}.pdf";

                        string urlPdf = await this.azureBlobService.UploadFileAsync(pdfData, fileName, container, "application/pdf");

                        await this.analiticasRepository.UpdateUrlDocumentoAnaliticaAsync(id, urlPdf);

                        return Ok(new { mensaje = "Analítica completada, resultados generados y PDF almacenado." });
                    }

                    return Ok(new { mensaje = "Analítica completada, pero hubo un error al generar el PDF." });
                }
                else
                {
                    return NotFound(new { mensaje = "Analítica no encontrada o no se pudo guardar los resultados." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno en el proceso de laboratorio: {ex.Message}" });
            }
        }
        #endregion
    }
}