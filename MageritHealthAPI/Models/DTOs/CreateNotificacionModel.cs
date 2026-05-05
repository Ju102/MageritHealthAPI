namespace MageritHealthAPI.Models.DTOs
{
    public class CreateNotificacionModel
    {
        public int IdUsuario { get; set; }

        public string Titulo { get; set; }

        public string Mensaje { get; set; }

        public string Tipo { get; set; }

        public string? EnlaceAccion { get; set; }

        public bool Leido { get; set; } = false;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
