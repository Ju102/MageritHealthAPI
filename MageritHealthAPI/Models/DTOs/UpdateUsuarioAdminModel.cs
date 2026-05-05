using System.ComponentModel.DataAnnotations;

namespace MageritHealthAPI.Models.DTOs
{
    public class UpdateUsuarioAdminModel
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido1 { get; set; }
        public string? Apellido2 { get; set; }
        public string? UrlImagen { get; set; }
        [Required]
        public string Dni { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Telefono { get; set; }
        public string Genero { get; set; }
        public string Direccion { get; set; }
        [Required]
        public string Email { get; set; }

        // Contacto de emergencia
        public string? ContactoEmergenciaNombre { get; set; }
        public string? ContactoEmergenciaTelefono { get; set; }

        // Campos que podrían editarse
        public int? IdEspecialidad { get; set; }
        public string? NumeroColegiado { get; set; }
        public string? NumeroAsegurado { get; set; }

        public bool Activo { get; set; }
    }
}