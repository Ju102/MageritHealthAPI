using MageritHealthAPI.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace MageritHealth.Services
{
    public class EmailingService : IEmailingService
    {
        private readonly IConfiguration config;

        public EmailingService(IConfiguration config)
        {
            this.config = config;
        }

        public async Task SendEmailRecuperacionAsync(string emailDestino, string nuevaPassword, string nombreUsuario)
        {
            string smtpHost = this.config["SmtpConfig:Host"];
            int smtpPort = int.Parse(this.config["SmtpConfig:Port"]);
            bool useSsl = bool.Parse(this.config["SmtpConfig:UseSsl"]);
            string smtpEmail = this.config["SmtpConfig:Email"];
            string smtpPassword = this.config["SmtpConfig:Password"];
            string displayName = this.config["SmtpConfig:DisplayName"];

            string htmlBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 10px; overflow: hidden;'>
                <div style='background-color: #0f4c81; padding: 20px; text-align: center; color: white;'>
                    <h2 style='margin: 0;'>Magerit Health</h2>
                </div>
                <div style='padding: 30px; color: #333;'>
                    <h3 style='color: #1e293b;'>Recuperación de contraseña</h3>
                    <p>Hola {nombreUsuario},</p>
                    <p>Hemos recibido una solicitud para restablecer el acceso a tu cuenta en nuestro portal de salud.</p>
                    <p>Tu nueva contraseña temporal es:</p>
                    
                    <div style='background-color: #f4f7f6; padding: 15px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 2px; border-radius: 5px; margin: 20px 0;'>
                        {nuevaPassword}
                    </div>
                    
                    <p>Te recomendamos que accedas a tu perfil y <strong>cambies esta contraseña</strong> por una nueva lo antes posible.</p>
                    <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;' />
                    <p style='color: #d9534f; font-size: 13px; font-weight: bold;'>
                        ⚠️ Aviso de seguridad: Si no has sido tú quien ha solicitado este cambio, por favor contacta con el soporte de Magerit Health inmediatamente.
                    </p>
                </div>
            </div>";

            using (var message = new MailMessage())
            {
                message.From = new MailAddress(smtpEmail, displayName);
                message.To.Add(new MailAddress(emailDestino));
                message.Subject = "Recuperación de contraseña - Magerit Health";
                message.Body = htmlBody;
                message.IsBodyHtml = true;

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.EnableSsl = useSsl;
                    client.Credentials = new NetworkCredential(smtpEmail, smtpPassword);
                    await client.SendMailAsync(message);
                }
            }
        }

        public async Task SendEmailBienvenidaAsync(string emailDestino, string passwordInicial, string nombreUsuario, string rolAsignado)
        {
            string smtpHost = this.config["SmtpConfig:Host"];
            int smtpPort = int.Parse(this.config["SmtpConfig:Port"]);
            bool useSsl = bool.Parse(this.config["SmtpConfig:UseSsl"]);
            string smtpEmail = this.config["SmtpConfig:Email"];
            string smtpPassword = this.config["SmtpConfig:Password"];
            string displayName = this.config["SmtpConfig:DisplayName"];

            string tipoPortal = rolAsignado.ToLower() switch
            {
                "paciente" => "Portal del Paciente",
                "doctor" => "Portal Médico",
                "admin" => "Centro de Control Administrativo",
                _ => "Portal de Magerit Health"
            };

            string htmlBody = $@"
                <div style='font-family: ""Segoe UI"", Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);'>
                    <div style='background-color: #0d6efd; padding: 25px; text-align: center; color: white;'>
                <h2 style='margin: 0; font-size: 24px; font-weight: 700; letter-spacing: 1px;'>Magerit Health</h2>
                <p style='margin: 5px 0 0 0; font-size: 14px; opacity: 0.9;'>Centro Médico Global</p>
                </div>
        
                <div style='padding: 35px; color: #334155; background-color: #ffffff;'>
                <h3 style='color: #0f172a; margin-top: 0; font-size: 20px;'>¡Te damos la bienvenida, {nombreUsuario}!</h3>
            
                <p style='line-height: 1.6;'>Tu cuenta ha sido creada con éxito en el sistema. A partir de ahora, tienes acceso al <strong>{tipoPortal}</strong>.</p>
                <p style='line-height: 1.6;'>Para iniciar sesión por primera vez, utiliza las siguientes credenciales:</p>
            
                <div style='background-color: #f8fafc; border-left: 4px solid #0d6efd; padding: 20px; border-radius: 0 8px 8px 0; margin: 25px 0;'>
                    <p style='margin: 0 0 10px 0; font-size: 15px;'><strong>Usuario (Email):</strong> {emailDestino}</p>
                    <p style='margin: 0; font-size: 15px;'><strong>Contraseña temporal:</strong></p>
                    <div style='font-size: 24px; font-weight: 800; letter-spacing: 3px; color: #0d6efd; margin-top: 8px;'>
                        {passwordInicial}
                    </div>
                </div>
            
                <p style='line-height: 1.6;'>Por tu seguridad, el sistema te solicitará o te recomendará <strong>cambiar esta contraseña</strong> la primera vez que accedas a tu perfil.</p>
            
                <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 30px 0;' />
            
                <p style='color: #64748b; font-size: 12px; text-align: center; margin: 0;'>
                    Este es un mensaje automático generado por Magerit Health. Por favor, no respondas a este correo.
                    </p>
                </div>
                </div>";

            using (var message = new MailMessage())
            {
                message.From = new MailAddress(smtpEmail, displayName);
                message.To.Add(new MailAddress(emailDestino));
                message.Subject = $"Bienvenido a Magerit Health - Credenciales de acceso";
                message.Body = htmlBody;
                message.IsBodyHtml = true;

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.EnableSsl = useSsl;
                    client.Credentials = new NetworkCredential(smtpEmail, smtpPassword);
                    await client.SendMailAsync(message);
                }
            }
        }
    }
}
