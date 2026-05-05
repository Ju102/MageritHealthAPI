using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MageritHealthAPI.Models
{
    [Table("MG_PRESCRIPCIONES")]
    public class Prescripcion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdPrescripcion { get; set; }

        public int IdCita { get; set; }
        public int IdMedicamento { get; set; }

        [MaxLength(100)]
        public string Instrucciones { get; set; }

        public DateTime FechaInicio { get; set; } = DateTime.UtcNow;
        public DateTime FechaFin { get; set; }

        public bool Activa { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        [ForeignKey("IdCita")]
        public Cita? Cita { get; set; }

        [JsonIgnore]
        [ForeignKey("IdMedicamento")]
        public Medicamento? Medicamento { get; set; }
    }
}