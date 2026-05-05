namespace MageritHealthAPI.Models.DTOs
{
    public class ClaimModel
    {
        public string IdUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Rol { get; set; }

        public string? NumeroAsegurado { get; set; }

        public string? NumeroColegiado { get; set; }
        public string? Especialidad { get; set; }
    }
}
