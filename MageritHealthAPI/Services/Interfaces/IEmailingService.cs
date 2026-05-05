namespace MageritHealthAPI.Services.Interfaces
{
    public interface IEmailingService
    {
        Task SendEmailRecuperacionAsync(string emailDestino, string nuevaPassword, string nombreUsuario);
        Task SendEmailBienvenidaAsync(string emailDestino, string passwordInicial, string nombreUsuario, string rolAsignado);
    }
}
