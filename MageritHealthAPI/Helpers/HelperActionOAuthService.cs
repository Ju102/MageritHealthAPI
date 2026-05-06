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
            // Se recuperan los valores de emisor, audiencia y clave secreta desde la configuración
            this.Issuer = config.GetValue<string>("ApiOAuthToken:Issuer");
            this.Audience = config.GetValue<string>("ApiOAuthToken:Audience");
            this.SecretKey = config.GetValue<string>("ApiOAuthToken:SecretKey");
        }

        public SymmetricSecurityKey GetKeyToken()
        {
            // Se convierte la clave secreta en un arreglo de bytes y se genera la clave de seguridad
            byte[] data = Encoding.UTF8.GetBytes(this.SecretKey);
            return new SymmetricSecurityKey(data);
        }

        public Action<JwtBearerOptions> GetJwtBearerOptions()
        {
            // Se definen las opciones de configuración para el middleware de JwtBearer
            Action<JwtBearerOptions> options = new Action<JwtBearerOptions>(options =>
            {
                // Se establecen los parámetros estándar para la validación del token
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

                // Se configura la interceptación del evento tras la validación de la firma
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // Se busca el claim encriptado que contiene los datos del usuario
                        var userDataClaim = context.Principal?.FindFirst("UserData")?.Value;

                        if (!string.IsNullOrEmpty(userDataClaim))
                        {
                            try
                            {
                                // Se desencripta la cadena obtenida mediante el helper de cifrado
                                string decryptedJson = CifradoHelper.DescifrarString(userDataClaim);

                                // Se realiza la deserialización del JSON al modelo de datos
                                var userInfo = JsonConvert.DeserializeObject<ClaimModel>(decryptedJson);

                                if (userInfo != null)
                                {
                                    // Se genera una lista de claims nativos de .NET para el Rol y el ID
                                    var claims = new List<Claim>
                                    {
                                        new Claim(ClaimTypes.Role, userInfo.Rol),
                                        new Claim(ClaimTypes.NameIdentifier, userInfo.IdUsuario)
                                    };

                                    // Se crea la nueva identidad y se añade al Principal de la petición actual
                                    var appIdentity = new ClaimsIdentity(claims);
                                    context.Principal?.AddIdentity(appIdentity);
                                }
                            }
                            catch (Exception)
                            {
                                // Se rechaza la petición en caso de fallo en la desencriptación o manipulación
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
            // Se configuran los esquemas de autenticación por defecto para la aplicación
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