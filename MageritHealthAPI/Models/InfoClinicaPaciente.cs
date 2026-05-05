using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MageritHealthAPI.Models
{
    [Table("MG_INFO_CLINICA_PACIENTES")]
    public class InfoClinicaPaciente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdInfoClinica { get; set; }

        public int IdPaciente { get; set; }

        [MaxLength(5)]
        public string GrupoSanguineo { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? PesoActual { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? EstaturaActual { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        [ForeignKey("IdPaciente")]
        public Usuario? Paciente { get; set; }
    }
}