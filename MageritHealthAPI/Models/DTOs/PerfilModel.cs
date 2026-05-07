namespace MageritHealthAPI.Models.DTOs
{
    public class PerfilModel
    {
        public string NombreCompleto { get; set; }
        
        public string Email { get; set; }

        public string? UrlImagen { get; set; }

        public string Dni { get; set; }

        public string Telefono { get; set; }

        public DateTime FechaNacimiento { get; set; }

        public string Genero { get; set; }

        public string? ContactoEmergenciaNombre { get; set; }

        public string? ContactoEmergenciaTelefono { get; set; }

        public string Direccion { get; set; }

        public DateTime FechaCreacion { get; set; }

        public string? Especialidad { get; set; }
        public string? NumeroColegiado { get; set; }

        public string? NumeroAsegurado { get; set; }
    }
}
