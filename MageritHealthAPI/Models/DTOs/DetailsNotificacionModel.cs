using System.ComponentModel.DataAnnotations;

namespace MageritHealthAPI.Models.DTOs
{
    public class DetailsNotificacionModel
    {
        public int IdNotificacion { get; set; }

        public string Titulo { get; set; }

        public string Mensaje { get; set; }

        public string Tipo { get; set; }

        public string? EnlaceAccion { get; set; }

        public bool Leido { get; set; }

        public DateTime FechaCreacion { get; set; }

    }
}
