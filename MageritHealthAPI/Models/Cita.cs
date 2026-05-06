using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MageritHealthAPI.Models
{
    [Table("MG_CITAS")]
    public class Cita
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdCita { get; set; }

        public int IdPaciente { get; set; }
        public int IdDoctor { get; set; }

        public string Motivo { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Notas { get; set; }

        [MaxLength(20)]
        public string Estado { get; set; } = "programada";

        [MaxLength(2048)]
        public string? UrlCita { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public bool Activa { get; set; } = true;

        [JsonIgnore]
        [ForeignKey("IdPaciente")]
        public Usuario? Paciente { get; set; }

        [JsonIgnore]
        [ForeignKey("IdDoctor")]
        public Usuario? Doctor { get; set; }

        [JsonIgnore]
        public List<Analitica> Analiticas { get; set; } = new List<Analitica>();

        [JsonIgnore]
        public List<Prescripcion> Prescripciones { get; set; } = new List<Prescripcion>();
    }
}