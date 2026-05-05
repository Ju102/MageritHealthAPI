using MageritHealthAPI.Helpers;
using MageritHealthAPI.Models;
using MageritHealthAPI.Models.DTOs;
using MageritHealthAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MageritHealthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUsuariosRepository usuariosRepository;
        private readonly HelperActionOAuthService authHelper;

        public AuthController(IUsuariosRepository usuariosRepository, HelperActionOAuthService authHelper)
        {
            this.usuariosRepository = usuariosRepository;
            this.authHelper = authHelper;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Login(LoginModel model)
        {
            try
            {
                Usuario usuario = await this.usuariosRepository.LoginUsuarioAsync(model.Email, model.Password);

                if (usuario == null)
                {
                    return Unauthorized();
                }

                SigningCredentials credenciales = new SigningCredentials(this.authHelper.GetKeyToken(), SecurityAlgorithms.HmacSha256);

                string apellido = usuario.Apellido2 != null ? $" {usuario.Apellido1} {usuario.Apellido2}" : $" {usuario.Apellido1}";

                ClaimModel userInfo = new ClaimModel()
                {
                    IdUsuario = usuario.IdUsuario.ToString(),
                    NombreCompleto = usuario.Nombre + apellido,
                    Rol = usuario.Rol
                };

                if (usuario.Rol == "paciente")
                {
                    userInfo.NumeroAsegurado = usuario.NumeroAsegurado;
                }

                if (usuario.Rol == "doctor")
                {
                    userInfo.NumeroColegiado = usuario.NumeroColegiado;
                    userInfo.Especialidad = usuario.Especialidad.NombreEspecialidad;
                }

                string userInfoJson = JsonConvert.SerializeObject(userInfo);
                string encryptedJson = CifradoHelper.CifrarString(userInfoJson);

                Claim[] info = new[]
                {
                new Claim("UserData", encryptedJson)
            };

                JwtSecurityToken token = new JwtSecurityToken(
                    claims: info,
                    issuer: this.authHelper.Issuer,
                    audience: this.authHelper.Audience,
                    signingCredentials: credenciales,
                    expires: DateTime.UtcNow.AddMinutes(10),
                    notBefore: DateTime.UtcNow
                );

                return Ok(
                    new
                    {
                        response = new JwtSecurityTokenHandler().WriteToken(token)
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al intentar iniciar sesión.", detalle = ex.Message });
            }
        }
    }
}
