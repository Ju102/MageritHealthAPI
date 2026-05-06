using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MageritHealthAPI.Models
{
    [Table("MG_ANALITICAS")]
    public class Analitica
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdAnalitica { get; set; }

        public int IdCita { get; set; }
        public DateTime FechaAnalitica { get; set; }

        [MaxLength(50)]
        public string Estado { get; set; } = "programada";

        public string? Notas { get; set; }

        [MaxLength(2048)]
        public string? UrlAnalitica { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        [ForeignKey("IdCita")]
        public Cita? Cita { get; set; }

        [JsonIgnore]
        public List<Medicion> Mediciones { get; set; } = new List<Medicion>();
    }
}