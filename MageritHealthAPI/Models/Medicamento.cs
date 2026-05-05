using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MageritHealthAPI.Models
{
    [Table("MG_MEDICAMENTOS")]
    public class Medicamento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdMedicamento { get; set; }

        [MaxLength(100)]
        public string NombreComercial { get; set; }

        [MaxLength(100)]
        public string PrincipioActivo { get; set; }

        [MaxLength(50)]
        public string Concentracion { get; set; }

        [MaxLength(50)]
        public string Formato { get; set; }

        [MaxLength(100)]
        public string Fabricante { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public List<Prescripcion> Prescripciones { get; set; } = new List<Prescripcion>();
    }
}