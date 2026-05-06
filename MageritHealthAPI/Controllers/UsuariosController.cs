using MageritHealthAPI.Models;
using MageritHealthAPI.Helpers;
using MageritHealthAPI.Models.DTOs;
using MageritHealthAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MageritHealthAPI.Services.Interfaces;

namespace MageritHealthAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuariosRepository usuariosRepository;
        private readonly IAzureBlobService azureBlobService;
        private readonly UserTokenHelper userTokenHelper;
        private readonly IConfiguration configuration;

        public UsuariosController(IUsuariosRepository usuariosRepository, IAzureBlobService azureBlobService, UserTokenHelper userTokenHelper, IConfiguration configuration)
        {
            this.usuariosRepository = usuariosRepository;
            this.azureBlobService = azureBlobService;
            this.userTokenHelper = userTokenHelper;
            this.configuration = configuration;
        }

        #region MAPPERS
        private DetailsUsuarioAdminViewModel MapToDTO(Usuario model)
        {
            return new DetailsUsuarioAdminViewModel
            {
                IdUsuario = model.IdUsuario,
                Nombre = model.Nombre,
                Apellido1 = model.Apellido1,
                Apellido2 = model.Apellido2,
                UrlImagen = model.UrlImagen,
                FechaNacimiento = model.FechaNacimiento,
                Telefono = model.Telefono,
                Genero = model.Genero,
                Direccion = model.Direccion,
                ContactoEmergenciaNombre = model.ContactoEmergenciaNombre,
                ContactoEmergenciaTelefono = model.ContactoEmergenciaTelefono,
                IdEspecialidad = model.IdEspecialidad,
                NumeroColegiado = model.NumeroColegiado,
                NumeroAsegurado = model.NumeroAsegurado,
                Email = model.Email,
                Dni = model.Dni,
                Rol = model.Rol,
                Activo = model.Activo,
                FechaCreacion = model.FechaCreacion,
            };
        }
        #endregion

        #region LECTURA (GET)
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<List<DetailsUsuarioAdminViewModel>>> Get([FromQuery] string? dni, [FromQuery] string? rol, [FromQuery] bool activos = true)
        {
            try
            {
                List<Usuario> usuarios = await this.usuariosRepository.GetUsuariosAsync(dni, rol, activos);
                return Ok(usuarios.Select(this.MapToDTO).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DetailsUsuarioAdminViewModel>> Get(int id)
        {
            try
            {
                Usuario usuario = await this.usuariosRepository.FindUsuarioByIdAsync(id);

                if (usuario == null)
                {
                    return NotFound(new { mensaje = "Usuario no encontrado." });
                }

                return Ok(this.MapToDTO(usuario));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<PerfilModel>> GetPerfil()
        {
            try
            {
                int id = this.userTokenHelper.GetUserId();
                Usuario usuario = await this.usuariosRepository.FindUsuarioByIdAsync(id);

                if (usuario == null) return NotFound(new { mensaje = "Usuario no encontrado." });

                string apellido = usuario.Apellido2 != null ? $"{usuario.Apellido1} {usuario.Apellido2}" : $"{usuario.Apellido1}";

                PerfilModel perfilModel = new PerfilModel()
                {
                    NombreCompleto = $"{usuario.Nombre} {apellido}".Trim(),
                    Email = usuario.Email,
                    Dni = usuario.Dni,
                    UrlImagen = usuario.UrlImagen,
                    Telefono = usuario.Telefono,
                    FechaNacimiento = usuario.FechaNacimiento,
                    Genero = usuario.Genero,
                    Direccion = usuario.Direccion,
                    Especialidad = usuario.Especialidad?.NombreEspecialidad,
                    NumeroAsegurado = usuario.NumeroAsegurado,
                    NumeroColegiado = usuario.NumeroColegiado,
                    FechaCreacion = usuario.FechaCreacion,
                };

                return Ok(perfilModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [Authorize(Roles = "doctor,admin")]
        [HttpGet]
        [Route("[action]/{iddoctor:int}")]
        public async Task<ActionResult<List<SeleccionUsuarioModel>>> GetPacientesDoctor(int iddoctor)
        {
            try
            {
                // SEGURIDAD: Un doctor no puede ver los pacientes de otro doctor
                var userInfo = this.userTokenHelper.GetInfoUser();
                int userId = int.Parse(userInfo.IdUsuario);

                if (userInfo.Rol.ToLower() == "doctor" && iddoctor != userId) return Forbid();

                List<Usuario> usuarios = await this.usuariosRepository.GetPacientesByIdDoctorAsync(iddoctor);
                List<SeleccionUsuarioModel> pacientes = new List<SeleccionUsuarioModel>();

                foreach (Usuario user in usuarios)
                {
                    string apellido = (user.Apellido2 == null) ? $"{user.Apellido1}" : $"{user.Apellido1} {user.Apellido2}";
                    pacientes.Add(new SeleccionUsuarioModel()
                    {
                        IdUsuario = user.IdUsuario,
                        NombreCompleto = $"{user.Nombre} {apellido}".Trim()
                    });
                }

                return Ok(pacientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("[action]/{idespecialidad:int}")]
        public async Task<ActionResult<List<SeleccionUsuarioModel>>> GetDoctoresByEspecialidad(int idespecialidad)
        {
            try
            {
                List<Usuario> usuarios = await this.usuariosRepository.GetDoctoresByEspecialidadAsync(idespecialidad);
                List<SeleccionUsuarioModel> doctores = new List<SeleccionUsuarioModel>();

                foreach (Usuario user in usuarios)
                {
                    string apellido = (user.Apellido2 == null) ? $"{user.Apellido1}" : $"{user.Apellido1} {user.Apellido2}";
                    doctores.Add(new SeleccionUsuarioModel()
                    {
                        IdUsuario = user.IdUsuario,
                        NombreCompleto = $"{user.Nombre} {apellido}".Trim()
                    });
                }

                return Ok(doctores);
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
        [Route("[action]")]
        public async Task<ActionResult> CreateAdmin([FromBody] CreateUsuarioModel admin)
        {
            try
            {
                Usuario usuario = new Usuario()
                {
                    Nombre = admin.Nombre,
                    Apellido1 = admin.Apellido1,
                    Apellido2 = admin.Apellido2,
                    Dni = admin.Dni,
                    Email = admin.Email,
                    Rol = "admin",
                    FechaNacimiento = admin.FechaNacimiento,
                    Genero = admin.Genero,
                    Telefono = admin.Telefono,
                    Direccion = admin.Direccion,
                    ContactoEmergenciaNombre = admin.NombreContactoEmergencia,
                    ContactoEmergenciaTelefono = admin.TelefonoContactoEmergencia,
                };

                bool creado = await this.usuariosRepository.InsertUsuarioAsync(usuario);

                if (creado) return CreatedAtAction(nameof(Get), new { mensaje = "Admin creado." });
                return BadRequest(new { mensaje = "No se pudo crear el admin." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> CreateDoctor([FromBody] CreateDoctorModel doctor)
        {
            try
            {
                Usuario usuario = new Usuario()
                {
                    Nombre = doctor.Nombre,
                    Apellido1 = doctor.Apellido1,
                    Apellido2 = doctor.Apellido2,
                    Dni = doctor.Dni,
                    Email = doctor.Email,
                    Rol = "doctor",
                    FechaNacimiento = doctor.FechaNacimiento,
                    Genero = doctor.Genero,
                    Telefono = doctor.Telefono,
                    Direccion = doctor.Direccion,
                    ContactoEmergenciaNombre = doctor.NombreContactoEmergencia,
                    ContactoEmergenciaTelefono = doctor.TelefonoContactoEmergencia,
                    IdEspecialidad = doctor.IdEspecialidad,
                    NumeroColegiado = doctor.NumeroColegiado
                };

                bool creado = await this.usuariosRepository.InsertUsuarioAsync(usuario);

                if (creado) return CreatedAtAction(nameof(Get), new { mensaje = "Doctor creado." });
                return BadRequest(new { mensaje = "No se pudo crear el doctor." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> CreatePaciente([FromBody] CreatePacienteModel paciente)
        {
            try
            {
                Usuario usuario = new Usuario()
                {
                    Nombre = paciente.Nombre,
                    Apellido1 = paciente.Apellido1,
                    Apellido2 = paciente.Apellido2,
                    Dni = paciente.Dni,
                    Email = paciente.Email,
                    Rol = "paciente",
                    FechaNacimiento = paciente.FechaNacimiento,
                    Genero = paciente.Genero,
                    Telefono = paciente.Telefono,
                    Direccion = paciente.Direccion,
                    ContactoEmergenciaNombre = paciente.NombreContactoEmergencia,
                    ContactoEmergenciaTelefono = paciente.TelefonoContactoEmergencia,
                    NumeroAsegurado = paciente.NumeroAsegurado
                };

                bool creado = await this.usuariosRepository.InsertUsuarioAsync(usuario);

                if (creado) return CreatedAtAction(nameof(Get), new { mensaje = "Paciente creado." });
                return BadRequest(new { mensaje = "No se pudo crear el paciente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("[action]/{id:int}")]
        public async Task<ActionResult> UpdateUsuario(int id, [FromBody] UpdateUsuarioAdminModel model)
        {
            try
            {
                Usuario usuarioUpdate = new Usuario
                {
                    IdUsuario = id,
                    Nombre = model.Nombre,
                    Apellido1 = model.Apellido1,
                    Apellido2 = model.Apellido2,
                    UrlImagen = model.UrlImagen,
                    FechaNacimiento = model.FechaNacimiento,
                    Telefono = model.Telefono,
                    Genero = model.Genero,
                    Direccion = model.Direccion,
                    ContactoEmergenciaNombre = model.ContactoEmergenciaNombre,
                    ContactoEmergenciaTelefono = model.ContactoEmergenciaTelefono,
                    IdEspecialidad = model.IdEspecialidad,
                    NumeroColegiado = model.NumeroColegiado,
                    NumeroAsegurado = model.NumeroAsegurado,
                    Email = model.Email,
                    Dni = model.Dni,
                    Activo = model.Activo
                };

                bool actualizado = await this.usuariosRepository.UpdateUsuarioAsync(usuarioUpdate);

                if (actualizado) return Ok(new { mensaje = "Usuario actualizado correctamente." });

                return NotFound(new { mensaje = "Usuario no encontrado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<ActionResult> UpdatePerfil([FromBody] UpdatePerfilModel model)
        {
            try
            {
                model.IdUsuario = this.userTokenHelper.GetUserId();
                bool actualizado = await this.usuariosRepository.UpdatePerfilUsuarioAsync(model);

                if (actualizado) return Ok(new { mensaje = "Perfil actualizado correctamente." });

                return BadRequest(new { mensaje = "No se pudo actualizar el perfil o las contraseñas no coinciden." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<ActionResult> SubirImagenPerfil(IFormFile imagen)
        {
            try
            {
                if (imagen == null || imagen.Length == 0)
                    return BadRequest(new { mensaje = "No se ha enviado ninguna imagen o el archivo está vacío." });

                int idUsuario = this.userTokenHelper.GetUserId();

                string extension = Path.GetExtension(imagen.FileName).ToLower();
                string nombreArchivo = $"perfil_{idUsuario}_{DateTime.UtcNow.Ticks}{extension}";

                string container = this.configuration["AzureStorageConfig:ContainerImagenes"];

                string urlImagenAzure = await this.azureBlobService.UploadFileAsync(imagen, nombreArchivo, container);

                bool actualizado = await this.usuariosRepository.UpdateUrlImagenUsuarioAsync(idUsuario, urlImagenAzure);

                if (actualizado)
                {
                    return Ok(new
                    {
                        mensaje = "Imagen de perfil actualizada correctamente.",
                        urlImagen = urlImagenAzure
                    });
                }

                return BadRequest(new { mensaje = "No se pudo actualizar el perfil en la base de datos." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("[action]/{id:int}")]
        public async Task<ActionResult> ResetPassword(int id)
        {
            try
            {
                bool success = await this.usuariosRepository.ResetPasswordUsuarioAsync(id);

                if (!success) return NotFound(new { mensaje = "Usuario no encontrado." });

                return Ok(new { mensaje = "Contraseña reseteada y enviada por correo electrónico." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("[action]/{id:int}")]
        public async Task<ActionResult> EnableUsuario(int id)
        {
            try
            {
                bool success = await this.usuariosRepository.UpdateEstadoUsuarioAsync(id, true);
                if (!success) return NotFound(new { mensaje = "Usuario no encontrado." });

                return Ok(new { mensaje = "Usuario activado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("[action]/{id:int}")]
        public async Task<ActionResult> DisableUsuario(int id)
        {
            try
            {
                bool success = await this.usuariosRepository.UpdateEstadoUsuarioAsync(id, false);
                if (!success) return NotFound(new { mensaje = "Usuario no encontrado." });

                return Ok(new { mensaje = "Usuario desactivado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }
        #endregion
    }
}