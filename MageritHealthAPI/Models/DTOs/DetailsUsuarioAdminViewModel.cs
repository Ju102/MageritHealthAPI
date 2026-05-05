namespace MageritHealthAPI.Models.DTOs
{
    public class DetailsUsuarioAdminViewModel
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido1 { get; set; }
        public string? Apellido2 { get; set; }
        public string? UrlImagen { get; set; }
        public string Dni { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Telefono { get; set; }
        public string Genero { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public string? ContactoEmergenciaNombre { get; set; }
        public string? ContactoEmergenciaTelefono { get; set; }
        public int? IdEspecialidad { get; set; }
        public string? NumeroColegiado { get; set; }
        public string? NumeroAsegurado { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
