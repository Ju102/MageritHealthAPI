// Asegúrate de importar el namespace donde tienes ClaimModel y CifradoHelper
using MageritHealthAPI.Models.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace MageritHealthAPI.Helpers
{
    public class HelperActionOAuthService
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }

        public HelperActionOAuthService(IConfiguration config)
        {
            this.Issuer = config.GetValue<string>("ApiOAuthToken:Issuer");
            this.Audience = config.GetValue<string>("ApiOAuthToken:Audience");
            this.SecretKey = config.GetValue<string>("ApiOAuthToken:SecretKey");
        }

        public SymmetricSecurityKey GetKeyToken()
        {
            byte[] data = Encoding.UTF8.GetBytes(this.SecretKey);
            return new SymmetricSecurityKey(data);
        }

        public Action<JwtBearerOptions> GetJwtBearerOptions()
        {
            Action<JwtBearerOptions> options = new Action<JwtBearerOptions>(options =>
            {
                // Parámetros de validación estándar
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = this.Issuer,
                    ValidAudience = this.Audience,
                    IssuerSigningKey = this.GetKeyToken()
                };

                // Intercepción del token tras validar la firma
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // Buscamos el claim encriptado que generaste en el AuthController
                        var userDataClaim = context.Principal?.FindFirst("UserData")?.Value;

                        if (!string.IsNullOrEmpty(userDataClaim))
                        {
                            try
                            {
                                // Desencriptamos el string (Asegúrate de que el método se llame así en tu CifradoHelper)
                                string decryptedJson = CifradoHelper.DescifrarString(userDataClaim);

                                // Deserializamos al modelo
                                var userInfo = JsonConvert.DeserializeObject<ClaimModel>(decryptedJson);

                                if (userInfo != null)
                                {
                                    // Creamos los Claims nativos de .NET para Rol e ID
                                    var claims = new List<Claim>
                                    {
                                        new Claim(ClaimTypes.Role, userInfo.Rol),
                                        new Claim(ClaimTypes.NameIdentifier, userInfo.IdUsuario)
                                    };

                                    // Añadimos esta nueva identidad al contexto de la petición actual
                                    var appIdentity = new ClaimsIdentity(claims);
                                    context.Principal?.AddIdentity(appIdentity);
                                }
                            }
                            catch (Exception)
                            {
                                // Si alguien manipula el string o falla la desencriptación, rechazamos la petición
                                context.Fail("El token contiene datos inválidos o corruptos.");
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            return options;
        }

        public Action<AuthenticationOptions> GetAuthenticationSchema()
        {
            Action<AuthenticationOptions> options = new Action<AuthenticationOptions>(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            return options;
        }
    }
}