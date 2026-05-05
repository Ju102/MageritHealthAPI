using System.ComponentModel.DataAnnotations;

namespace MageritHealthAPI.Models.DTOs
{
    public class CreateUsuarioModel
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido1 { get; set; }
        public string? Apellido2 { get; set; }
        [Required]
        public string Dni { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public DateTime FechaNacimiento { get; set; }
        [Required]
        public string Genero { get; set; }
        [Required]
        public string Telefono { get; set; }
        [Required]
        public string Direccion { get; set; }

        public string? NombreContactoEmergencia { get; set; }
        public string? TelefonoContactoEmergencia { get; set; }
    }
}