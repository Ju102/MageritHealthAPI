using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MageritHealthAPI.Models
{
    [Table("MG_ANTECEDENTES_MEDICOS")]
    public class AntecedenteMedico
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdAntecedente { get; set; }

        public int IdPaciente { get; set; }

        [MaxLength(50)]
        public string Tipo { get; set; }

        [MaxLength(100)]
        public string Nombre { get; set; }

        [MaxLength(20)]
        public string? Severidad { get; set; }

        public DateTime? FechaDiagnostico { get; set; }

        public string? Notas { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime? FechaRegistro { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        [ForeignKey("IdPaciente")]
        public Usuario? Paciente { get; set; }
    }
}