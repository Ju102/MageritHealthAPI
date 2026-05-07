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
    public class MedicamentosController : ControllerBase
    {
        private readonly IMedicamentosRepository medicamentosRepository;

        public MedicamentosController(IMedicamentosRepository medicamentosRepository)
        {
            this.medicamentosRepository = medicamentosRepository;
        }

        #region MAPPERS
        private DetailsMedicamentoModel MapToDetails(Medicamento m)
        {
            return new DetailsMedicamentoModel
            {
                IdMedicamento = m.IdMedicamento,
                NombreComercial = m.NombreComercial,
                Fabricante = m.Fabricante,
                PrincipioActivo = m.PrincipioActivo,
                Concentracion = m.Concentracion,
                Formato = m.Formato,
                FechaCreacion = m.FechaCreacion,
                Activo = m.Activo
            };
        }
        #endregion

        #region LECTURA (GET)
        [HttpGet]
        public async Task<ActionResult<List<DetailsMedicamentoModel>>> Get()
        {
            try
            {
                List<Medicamento> medicamentos = await this.medicamentosRepository.GetMedicamentosAsync();
                return Ok(medicamentos.Select(MapToDetails).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DetailsMedicamentoModel>> Get(int id)
        {
            try
            {
                Medicamento medicamento = await this.medicamentosRepository.GetMedicamentoByIdAsync(id);
                if (medicamento == null)
                {
                    return NotFound(new { mensaje = $"No se encontró el medicamento con ID {id}." });
                }
                return Ok(MapToDetails(medicamento));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("[action]/{nombre}")]
        public async Task<ActionResult<List<DetailsMedicamentoModel>>> Search(string nombre)
        {
            try
            {
                List<Medicamento> medicamentos = await this.medicamentosRepository.SearchMedicamentosAsync(nombre);
                return Ok(medicamentos.Select(MapToDetails).ToList());
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
        public async Task<ActionResult> Post([FromBody] CreateMedicamentoModel model)
        {
            try
            {
                if (model == null || !ModelState.IsValid)
                {
                    return BadRequest(new { mensaje = "El modelo de medicamento no es válido." });
                }

                Medicamento medicamento = new Medicamento
                {
                    NombreComercial = model.NombreComercial,
                    Fabricante = model.Fabricante,
                    PrincipioActivo = model.PrincipioActivo,
                    Concentracion = model.Concentracion,
                    Formato = model.Formato,
                    Activo = true
                };

                bool creado = await this.medicamentosRepository.CreateMedicamentoAsync(medicamento);

                if (creado)
                {
                    return Ok(new { mensaje = "Medicamento creado con éxito." });
                }

                return BadRequest(new { mensaje = "No se ha podido guardar el medicamento en la base de datos." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] UpdateMedicamentoModel model)
        {
            try
            {
                if (model == null || !ModelState.IsValid)
                {
                    return BadRequest(new { mensaje = "El modelo de medicamento no es válido." });
                }

                if (id != model.IdMedicamento)
                {
                    return BadRequest(new { mensaje = "El ID de la ruta no coincide con el del modelo." });
                }

                Medicamento medicamento = new Medicamento
                {
                    IdMedicamento = id,
                    NombreComercial = model.NombreComercial,
                    Fabricante = model.Fabricante,
                    PrincipioActivo = model.PrincipioActivo,
                    Concentracion = model.Concentracion,
                    Formato = model.Formato
                };

                bool actualizado = await this.medicamentosRepository.UpdateMedicamentoAsync(medicamento);

                if (actualizado)
                {
                    return Ok(new { mensaje = "Medicamento actualizado correctamente." });
                }

                return NotFound(new { mensaje = "Medicamento no encontrado o no hubo cambios." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("[action]/{id:int}")]
        public async Task<ActionResult> EnableMedicamento(int id)
        {
            try
            {
                bool activado = await this.medicamentosRepository.UpdateEstadoMedicamentoAsync(id, true);
                if (activado) return Ok(new { mensaje = "Medicamento activado correctamente." });

                return NotFound(new { mensaje = "Medicamento no encontrado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("[action]/{id:int}")]
        public async Task<ActionResult> DisableMedicamento(int id)
        {
            try
            {
                bool desactivado = await this.medicamentosRepository.UpdateEstadoMedicamentoAsync(id, false);
                if (desactivado) return Ok(new { mensaje = "Medicamento desactivado correctamente." });

                return NotFound(new { mensaje = "Medicamento no encontrado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }
        #endregion
    }
}