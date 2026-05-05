using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MageritHealthAPI.Models
{
    [Table("MG_NOTIFICACIONES")]
    public class Notificacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdNotificacion { get; set; }

        public int IdUsuario { get; set; }

        [MaxLength(50)]
        public string Titulo { get; set; }

        public string Mensaje { get; set; }

        [MaxLength(20)]
        public string Tipo { get; set; }

        public string? EnlaceAccion { get; set; }

        public bool Leido { get; set; } = false;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }
    }
}